namespace ClassFramework.Pipelines.Extensions;

public static class PipelineContextExtensions
{
    public static Result<string> CreateEntityInstanciation(this PipelineContext<IConcreteTypeBuilder, BuilderContext> context, IFormattableStringParser formattableStringParser, string classNameSuffix)
    {
        formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));

        var customEntityInstanciation = context.Context.SourceModel.Metadata.GetStringValue(MetadataNames.CustomBuilderEntityInstanciation);
        if (!string.IsNullOrEmpty(customEntityInstanciation))
        {
            return formattableStringParser.Parse(customEntityInstanciation, context.Context.FormatProvider, context);
        }

        if (context.Context.SourceModel is not IConstructorsContainer constructorsContainer)
        {
            return Result.Invalid<string>("Cannot create an instance of a type that does not have constructors");
        }

        if (context.Context.SourceModel is Class cls && cls.Abstract)
        {
            return Result.Invalid<string>("Cannot create an instance of an abstract class");
        }

        var hasPublicParameterlessConstructor = constructorsContainer.HasPublicParameterlessConstructor();
        var openSign = GetBuilderPocoOpenSign(hasPublicParameterlessConstructor && context.Context.SourceModel.Properties.Count != 0);
        var closeSign = GetBuilderPocoCloseSign(hasPublicParameterlessConstructor && context.Context.SourceModel.Properties.Count != 0);

        var parametersResult = GetConstructionMethodParameters(context, formattableStringParser, hasPublicParameterlessConstructor);
        if (!parametersResult.IsSuccessful())
        {
            return parametersResult;
        }

        var entityNamespace = context.Context.SourceModel.Metadata.WithMappingMetadata(context.Context.SourceModel.GetFullName().GetCollectionItemType().WhenNullOrEmpty(context.Context.SourceModel.GetFullName), context.Context.Settings).GetStringValue(MetadataNames.CustomEntityNamespace, () => context.Context.SourceModel.Namespace);
        var ns = context.Context.MapNamespace(entityNamespace).AppendWhenNotNullOrEmpty(".");

        return Result.Success($"new {ns}{context.Context.SourceModel.Name}{classNameSuffix}{context.Context.SourceModel.GetGenericTypeArgumentsString()}{openSign}{parametersResult.Value}{closeSign}");
    }

    public static string CreateEntityChainCall<TModel>(this PipelineContext<TModel, EntityContext> context, bool baseClass)
    {
        context = context.IsNotNull(nameof(context));

        if (baseClass && context.Context.Settings.AddValidationCode() == ArgumentValidationType.Shared)
        {
            return $"base({CreateImmutableClassCtorParameterNames(context)})";
        }

        return context.Context.Settings.EnableInheritance && context.Context.Settings.BaseClass is not null
            ? $"base({GetPropertyNamesConcatenated(context.Context.Settings.BaseClass.Properties, context.Context.FormatProvider.ToCultureInfo())})"
            : context.Context.SourceModel.GetCustomValueForInheritedClass(context.Context.Settings.EnableInheritance,
            cls => Result.Success($"base({GetPropertyNamesConcatenated(context.Context.SourceModel.Properties.Where(x => x.ParentTypeFullName == cls.BaseClass), context.Context.FormatProvider.ToCultureInfo())})")).Value!; // we can simply shortcut the result evaluation, because we are injecting the Success in the delegate
    }

    private static string GetPropertyNamesConcatenated(IEnumerable<Property> properties, CultureInfo cultureInfo)
        => string.Join(", ", properties.Select(x => x.Name.ToPascalCase(cultureInfo).GetCsharpFriendlyName()));

    private static string CreateImmutableClassCtorParameterNames<TModel>(PipelineContext<TModel, EntityContext> context)
        => string.Join(", ", context.Context.SourceModel.Properties.CreateImmutableClassCtorParameters(context.Context.FormatProvider, context.Context.Settings, n => context.Context.MapTypeName(n)).Select(x => x.Name.GetCsharpFriendlyName()));

    private static Result<string> GetConstructionMethodParameters<TModel>(PipelineContext<TModel, BuilderContext> context, IFormattableStringParser formattableStringParser, bool hasPublicParameterlessConstructor)
    {
        var properties = context.Context.SourceModel.GetBuilderConstructorProperties(context.Context);

        var results = properties.Select
        (
            property => new
            {
                property.Name,
                Source = property,
                Result = formattableStringParser.Parse
                (
                    property.Metadata
                        .WithMappingMetadata
                        (
                            property.TypeName.GetCollectionItemType().WhenNullOrEmpty(property.TypeName),
                            context.Context.Settings
                        ).GetStringValue(MetadataNames.CustomBuilderMethodParameterExpression, "[Name]"),
                    context.Context.FormatProvider,
                    new ParentChildContext<PipelineContext<TModel, BuilderContext>, Property>(context, property, context.Context.Settings)
                ),
                CollectionInitializer = property.Metadata
                    .WithMappingMetadata
                    (
                        property.TypeName.FixTypeName().WithoutProcessedGenerics(), // i.e. List<> etc.
                        context.Context.Settings
                    ).GetStringValue(MetadataNames.CustomCollectionInitialization, () => "[Expression]"),
                Suffix = property.GetSuffix(context.Context.Settings.EnableNullableReferenceTypes)
            }
        ).TakeWhileWithFirstNonMatching(x => x.Result.IsSuccessful()).ToArray();

        var error = Array.Find(results, x => !x.Result.IsSuccessful());
        if (error is not null)
        {
            return error.Result;
        }

        return Result.Success(string.Join(", ", results.Select(x => hasPublicParameterlessConstructor
            ? $"{x.Name} = {GetBuilderPropertyExpression(x.Result.Value, x.Source, x.CollectionInitializer, x.Suffix)}"
            : GetBuilderPropertyExpression(x.Result.Value, x.Source, x.CollectionInitializer, x.Suffix))));
    }

    private static string? GetBuilderPropertyExpression(this string? value, Property sourceProperty, string collectionInitializer, string suffix)
    {
        if (value is null || !value.Contains("[Name]"))
        {
            return value;
        }

        if (value == "[Name]")
        {
            return sourceProperty.Name;
        }

        if (sourceProperty.TypeName.FixTypeName().IsCollectionTypeName())
        {
            return GetCollectionBuilderPropertyExpression(value, sourceProperty, collectionInitializer, suffix);
        }
        else
        {
            return value!.Replace("[Name]", sourceProperty.Name).Replace("[NullableSuffix]", suffix);
        }
    }

    private static string GetCollectionBuilderPropertyExpression(string? value, Property sourceProperty, string collectionInitializer, string suffix)
        => collectionInitializer
            .Replace("[Type]", sourceProperty.TypeName.FixTypeName().WithoutProcessedGenerics())
            .Replace("[Generics]", sourceProperty.TypeName.FixTypeName().GetGenericArguments())
            .Replace("[Expression]", $"{sourceProperty.Name}{suffix}.Select(x => {value!.Replace("[Name]", "x").Replace("[NullableSuffix]", string.Empty)})");

    private static string GetBuilderPocoCloseSign(bool poco)
        => poco
            ? " }"
            : ")";

    private static string GetBuilderPocoOpenSign(bool poco)
        => poco
            ? " { "
            : "(";
}

namespace ClassFramework.Pipelines.Extensions;

public static class PipelineContextExtensions
{
    public static Result<FormattableStringParserResult> CreateEntityInstanciation(this PipelineContext<IConcreteTypeBuilder, BuilderContext> context, IFormattableStringParser formattableStringParser, ICsharpExpressionDumper csharpExpressionDumper, string classNameSuffix)
    {
        formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));

        var customEntityInstanciation = context.Context
            .GetMappingMetadata(context.Context.SourceModel.GetFullName())
            .GetStringValue(MetadataNames.CustomBuilderEntityInstanciation);
        if (!string.IsNullOrEmpty(customEntityInstanciation))
        {
            return formattableStringParser.Parse(customEntityInstanciation, context.Context.FormatProvider, context);
        }

        if (context.Context.SourceModel is not IConstructorsContainer constructorsContainer)
        {
            return Result.Invalid<FormattableStringParserResult>("Cannot create an instance of a type that does not have constructors");
        }

        if (context.Context.SourceModel is Class cls && cls.Abstract)
        {
            return Result.Invalid<FormattableStringParserResult>("Cannot create an instance of an abstract class");
        }

        var hasPublicParameterlessConstructor = constructorsContainer.HasPublicParameterlessConstructor();
        var openSign = GetBuilderPocoOpenSign(hasPublicParameterlessConstructor && context.Context.SourceModel.Properties.Count != 0);
        var closeSign = GetBuilderPocoCloseSign(hasPublicParameterlessConstructor && context.Context.SourceModel.Properties.Count != 0);

        var parametersResult = GetConstructionMethodParameters(context, formattableStringParser, csharpExpressionDumper, hasPublicParameterlessConstructor);
        if (!parametersResult.IsSuccessful())
        {
            return parametersResult;
        }

        var entityNamespace = context.Context.GetMappingMetadata(context.Context.SourceModel.GetFullName()).GetStringValue(MetadataNames.CustomEntityNamespace, () => context.Context.SourceModel.Namespace);
        var ns = context.Context.MapNamespace(entityNamespace).AppendWhenNotNullOrEmpty(".");

        return Result.Success($"new {ns}{context.Context.SourceModel.Name}{classNameSuffix}{context.Context.SourceModel.GetGenericTypeArgumentsString()}{openSign}{parametersResult.Value}{closeSign}".ToFormattableStringParserResult());
    }

    public static string CreateEntityChainCall<TModel>(this PipelineContext<TModel, EntityContext> context)
    {
        context = context.IsNotNull(nameof(context));

        return context.Context.Settings.EnableInheritance && context.Context.Settings.BaseClass is not null
            ? $"base({GetPropertyNamesConcatenated(context.Context.Settings.BaseClass.Properties, context.Context.FormatProvider.ToCultureInfo())})"
            : context.Context.SourceModel.GetCustomValueForInheritedClass(context.Context.Settings.EnableInheritance,
            cls => Result.Success($"base({GetPropertyNamesConcatenated(context.Context.SourceModel.Properties.Where(x => x.ParentTypeFullName == cls.BaseClass), context.Context.FormatProvider.ToCultureInfo())})")).Value!; // we can simply shortcut the result evaluation, because we are injecting the Success in the delegate
    }

    private static string GetPropertyNamesConcatenated(IEnumerable<Property> properties, CultureInfo cultureInfo)
        => string.Join(", ", properties.Select(x => x.Name.ToPascalCase(cultureInfo).GetCsharpFriendlyName()));

    private static Result<FormattableStringParserResult> GetConstructionMethodParameters<TModel>(PipelineContext<TModel, BuilderContext> context, IFormattableStringParser formattableStringParser, ICsharpExpressionDumper csharpExpressionDumper, bool hasPublicParameterlessConstructor)
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
                    context.Context
                        .GetMappingMetadata(property.TypeName)
                        .GetStringValue(MetadataNames.CustomBuilderMethodParameterExpression, PlaceholderNames.NamePlaceholder),
                    context.Context.FormatProvider,
                    new ParentChildContext<PipelineContext<TModel, BuilderContext>, Property>(context, property, context.Context.Settings)
                ),
                CollectionInitializer = context.Context.GetMappingMetadata
                    (
                        property.TypeName.FixTypeName().WithoutProcessedGenerics() // i.e. List<> etc.
                    ).GetStringValue(MetadataNames.CustomCollectionInitialization, () => "[Expression]"),
                Suffix = property.GetSuffix(context.Context.Settings.EnableNullableReferenceTypes, csharpExpressionDumper, context.Context)
            }
        ).TakeWhileWithFirstNonMatching(x => x.Result.IsSuccessful()).ToArray();

        var error = Array.Find(results, x => !x.Result.IsSuccessful());
        if (error is not null)
        {
            return error.Result;
        }

        return Result.Success(string.Join(", ", results.Select(x => hasPublicParameterlessConstructor
            ? $"{x.Name} = {GetBuilderPropertyExpression(x.Result.Value!, x.Source, x.CollectionInitializer, x.Suffix)}"
            : GetBuilderPropertyExpression(x.Result.Value!, x.Source, x.CollectionInitializer, x.Suffix))))
                .Transform(x => x.ToFormattableStringParserResult());
    }

    private static string? GetBuilderPropertyExpression(this string? value, Property sourceProperty, string collectionInitializer, string suffix)
    {
        if (value is null || !value.Contains(PlaceholderNames.NamePlaceholder))
        {
            return value;
        }

        if (value == PlaceholderNames.NamePlaceholder)
        {
            return sourceProperty.Name;
        }

        if (sourceProperty.TypeName.FixTypeName().IsCollectionTypeName())
        {
            return GetCollectionBuilderPropertyExpression(value, sourceProperty, collectionInitializer, suffix);
        }
        else
        {
            return value!
                .Replace(PlaceholderNames.NamePlaceholder, sourceProperty.Name)
                .Replace("[NullableSuffix]", suffix)
                .Replace("[ForcedNullableSuffix]", string.IsNullOrEmpty(suffix) ? string.Empty : "!");
        }
    }

    private static string GetCollectionBuilderPropertyExpression(string? value, Property sourceProperty, string collectionInitializer, string suffix)
        => collectionInitializer
            .Replace("[Type]", sourceProperty.TypeName.FixTypeName().WithoutProcessedGenerics())
            .Replace("[Generics]", sourceProperty.TypeName.FixTypeName().GetProcessedGenericArguments(addBrackets: true))
            .Replace("[Expression]", $"{sourceProperty.Name}{suffix}.Select(x => {value!.Replace(PlaceholderNames.NamePlaceholder, "x").Replace("[NullableSuffix]", string.Empty).Replace("[ForcedNullableSuffix]", sourceProperty.IsValueType ? string.Empty : "!")})");

    private static string GetBuilderPocoCloseSign(bool poco)
        => poco
            ? " }"
            : ")";

    private static string GetBuilderPocoOpenSign(bool poco)
        => poco
            ? " { "
            : "(";
}

namespace ClassFramework.Pipelines.Extensions;

public static class PipelineContextExtensions
{
    public static Result<FormattableStringParserResult> CreateEntityInstanciation(this PipelineContext<BuilderContext> context, IFormattableStringParser formattableStringParser, ICsharpExpressionDumper csharpExpressionDumper, string classNameSuffix)
    {
        formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));

        var customEntityInstanciation = context.Request
            .GetMappingMetadata(context.Request.SourceModel.GetFullName())
            .GetStringValue(MetadataNames.CustomBuilderEntityInstanciation);
        if (!string.IsNullOrEmpty(customEntityInstanciation))
        {
            return formattableStringParser.Parse(customEntityInstanciation, context.Request.FormatProvider, context);
        }

        if (context.Request.SourceModel is not IConstructorsContainer constructorsContainer)
        {
            return Result.Invalid<FormattableStringParserResult>("Cannot create an instance of a type that does not have constructors");
        }

        if (context.Request.SourceModel is Class cls && cls.Abstract)
        {
            return Result.Invalid<FormattableStringParserResult>("Cannot create an instance of an abstract class");
        }

        var hasPublicParameterlessConstructor = constructorsContainer.HasPublicParameterlessConstructor();
        var openSign = GetBuilderPocoOpenSign(hasPublicParameterlessConstructor && context.Request.SourceModel.Properties.Count != 0);
        var closeSign = GetBuilderPocoCloseSign(hasPublicParameterlessConstructor && context.Request.SourceModel.Properties.Count != 0);

        var parametersResult = GetConstructionMethodParameters(context, formattableStringParser, csharpExpressionDumper, hasPublicParameterlessConstructor);
        if (!parametersResult.IsSuccessful())
        {
            return parametersResult;
        }

        var entityNamespace = context.Request.GetMappingMetadata(context.Request.SourceModel.GetFullName()).GetStringValue(MetadataNames.CustomEntityNamespace, () => context.Request.SourceModel.Namespace);
        var ns = context.Request.MapNamespace(entityNamespace).AppendWhenNotNullOrEmpty(".");

        return Result.Success<FormattableStringParserResult>($"new {ns}{context.Request.SourceModel.Name}{classNameSuffix}{context.Request.SourceModel.GetGenericTypeArgumentsString()}{openSign}{parametersResult.Value}{closeSign}");
    }

    public static string CreateEntityChainCall(this PipelineContext<EntityContext> context)
    {
        context = context.IsNotNull(nameof(context));

        return context.Request.Settings.EnableInheritance && context.Request.Settings.BaseClass is not null
            ? $"base({GetPropertyNamesConcatenated(context.Request.Settings.BaseClass.Properties, context.Request.FormatProvider.ToCultureInfo())})"
            : context.Request.SourceModel.GetCustomValueForInheritedClass(context.Request.Settings.EnableInheritance,
            cls => Result.Success<FormattableStringParserResult>($"base({GetPropertyNamesConcatenated(context.Request.SourceModel.Properties.Where(x => x.ParentTypeFullName == cls.BaseClass), context.Request.FormatProvider.ToCultureInfo())})")).Value!; // we can simply shortcut the result evaluation, because we are injecting the Success in the delegate
    }

    private static string GetPropertyNamesConcatenated(IEnumerable<Property> properties, CultureInfo cultureInfo)
        => string.Join(", ", properties.Select(x => x.Name.ToCamelCase(cultureInfo).GetCsharpFriendlyName()));

    private static Result<FormattableStringParserResult> GetConstructionMethodParameters(PipelineContext<BuilderContext> context, IFormattableStringParser formattableStringParser, ICsharpExpressionDumper csharpExpressionDumper, bool hasPublicParameterlessConstructor)
    {
        var properties = context.Request.SourceModel.GetBuilderConstructorProperties(context.Request);

        var results = properties.Select
        (
            property => new
            {
                property.Name,
                Source = property,
                Result = formattableStringParser.Parse
                (
                    context.Request
                        .GetMappingMetadata(property.TypeName)
                        .GetStringValue(MetadataNames.CustomBuilderMethodParameterExpression, PlaceholderNames.NamePlaceholder),
                    context.Request.FormatProvider,
                    new ParentChildContext<PipelineContext<BuilderContext>, Property>(context, property, context.Request.Settings)
                ),
                CollectionInitializer = context.Request.GetMappingMetadata
                    (
                        property.TypeName.FixTypeName().WithoutProcessedGenerics() // i.e. List<> etc.
                    ).GetStringValue(MetadataNames.CustomCollectionInitialization, () => "[Expression]"),
                Suffix = property.GetSuffix(context.Request.Settings.EnableNullableReferenceTypes, csharpExpressionDumper, context.Request)
            }
        ).TakeWhileWithFirstNonMatching(x => x.Result.IsSuccessful()).ToArray();

        var error = Array.Find(results, x => !x.Result.IsSuccessful());
        if (error is not null)
        {
            return error.Result;
        }

        return Result.Success<FormattableStringParserResult>(string.Join(", ", results.Select(x => hasPublicParameterlessConstructor
            ? $"{x.Name} = {GetBuilderPropertyExpression(x.Result.Value!, x.Source, x.CollectionInitializer, x.Suffix)}"
            : GetBuilderPropertyExpression(x.Result.Value!, x.Source, x.CollectionInitializer, x.Suffix))));
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

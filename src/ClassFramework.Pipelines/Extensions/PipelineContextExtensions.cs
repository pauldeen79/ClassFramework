namespace ClassFramework.Pipelines.Extensions;

public static class PipelineContextExtensions
{
    public static async Task<Result<GenericFormattableString>> CreateEntityInstanciationAsync(this PipelineContext<BuilderContext> context, IExpressionEvaluator evaluator, ICsharpExpressionDumper csharpExpressionDumper, string classNameSuffix, CancellationToken token)
    {
        evaluator = evaluator.IsNotNull(nameof(evaluator));

        var customEntityInstanciation = context.Request
            .GetMappingMetadata(context.Request.SourceModel.GetFullName())
            .GetStringValue(MetadataNames.CustomBuilderEntityInstanciation);
        if (!string.IsNullOrEmpty(customEntityInstanciation))
        {
            return await evaluator.EvaluateInterpolatedStringAsync(customEntityInstanciation, context.Request.FormatProvider, context.Request, token).ConfigureAwait(false);
        }

        if (context.Request.SourceModel is not IConstructorsContainer constructorsContainer)
        {
            return Result.Invalid<GenericFormattableString>("Cannot create an instance of a type that does not have constructors");
        }

        if (context.Request.SourceModel is Class cls && cls.Abstract)
        {
            return Result.Invalid<GenericFormattableString>("Cannot create an instance of an abstract class");
        }

        var hasPublicParameterlessConstructor = constructorsContainer.HasPublicParameterlessConstructor();
        var openSign = GetBuilderPocoOpenSign(hasPublicParameterlessConstructor && context.Request.SourceModel.Properties.Count != 0);
        var closeSign = GetBuilderPocoCloseSign(hasPublicParameterlessConstructor && context.Request.SourceModel.Properties.Count != 0);

        var parametersResult = await GetConstructionMethodParametersAsync(context, evaluator, csharpExpressionDumper, hasPublicParameterlessConstructor, token).ConfigureAwait(false);
        if (!parametersResult.IsSuccessful())
        {
            return parametersResult;
        }

        var entityNamespace = context.Request.GetMappingMetadata(context.Request.SourceModel.GetFullName()).GetStringValue(MetadataNames.CustomEntityNamespace, () => context.Request.SourceModel.Namespace);
        var ns = context.Request.MapNamespace(entityNamespace).AppendWhenNotNullOrEmpty(".");

        return Result.Success<GenericFormattableString>($"new {ns}{context.Request.SourceModel.Name}{classNameSuffix}{context.Request.SourceModel.GetGenericTypeArgumentsString()}{openSign}{parametersResult.Value}{closeSign}");
    }

    public static async Task<string> CreateEntityChainCallAsync(this PipelineContext<EntityContext> context)
    {
        context = context.IsNotNull(nameof(context));

        return context.Request.Settings.EnableInheritance && context.Request.Settings.BaseClass is not null
            ? $"base({GetPropertyNamesConcatenated(context.Request.Settings.BaseClass.Properties, context.Request.FormatProvider.ToCultureInfo())})"
            : (await context.Request.SourceModel.GetCustomValueForInheritedClassAsync(context.Request.Settings.EnableInheritance,
                cls => Task.FromResult(Result.Success<GenericFormattableString>($"base({GetPropertyNamesConcatenated(context.Request.SourceModel.Properties.Where(x => x.ParentTypeFullName == cls.BaseClass), context.Request.FormatProvider.ToCultureInfo())})"))).ConfigureAwait(false)).Value!; // we can simply shortcut the result evaluation, because we are injecting the Success in the delegate
    }

    private static async Task<Result<GenericFormattableString>> GetConstructionMethodParametersAsync(PipelineContext<BuilderContext> context, IExpressionEvaluator evaluator, ICsharpExpressionDumper csharpExpressionDumper, bool hasPublicParameterlessConstructor, CancellationToken token)
    {
        var properties = context.Request.SourceModel.GetBuilderConstructorProperties(context.Request);

        var results = new List<ConstructionMethodParameterInfo>();
        foreach (var property in properties)
        {
            var info =
                new ConstructionMethodParameterInfo(
                    property.Name,
                    property,
                    await evaluator.EvaluateInterpolatedStringAsync
                    (
                        context.Request
                            .GetMappingMetadata(property.TypeName)
                            .GetStringValue(MetadataNames.CustomBuilderMethodParameterExpression, PlaceholderNames.NamePlaceholder),
                        context.Request.FormatProvider,
                        new ParentChildContext<PipelineContext<BuilderContext>, Property>(context, property, context.Request.Settings),
                        token
                    ).ConfigureAwait(false),
                    context.Request.GetMappingMetadata
                    (
                        property.TypeName.FixTypeName().WithoutGenerics() // i.e. List<> etc.
                    ).GetStringValue(MetadataNames.CustomCollectionInitialization, () => "[Expression]"),
                    property.GetSuffix(context.Request.Settings.EnableNullableReferenceTypes, csharpExpressionDumper, context.Request)
                );

            results.Add(info);

            if (!info.Result.IsSuccessful())
            {
                break;
            }
        }

        var error = results.Find(x => !x.Result.IsSuccessful());
        if (error is not null)
        {
            return error.Result;
        }

        return Result.Success<GenericFormattableString>(string.Join(", ", results.Select(x => hasPublicParameterlessConstructor
            ? $"{x.Name} = {GetBuilderPropertyExpression(x.Result.Value!, x.Source, x.CollectionInitializer, x.Suffix)}"
            : GetBuilderPropertyExpression(x.Result.Value!, x.Source, x.CollectionInitializer, x.Suffix))));
    }

    private static string GetPropertyNamesConcatenated(IEnumerable<Property> properties, CultureInfo cultureInfo)
        => string.Join(", ", properties.Select(x => x.Name.ToCamelCase(cultureInfo).GetCsharpFriendlyName()));

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
            .Replace("[Type]", sourceProperty.TypeName.FixTypeName().WithoutGenerics())
            .Replace("[Generics]", sourceProperty.TypeName.FixTypeName().GetGenericArguments(addBrackets: true))
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

internal class ConstructionMethodParameterInfo
{
    public string Name { get; }
    public Property Source { get; }
    public Result<GenericFormattableString> Result { get; }
    public string CollectionInitializer { get; }
    public string Suffix { get; }

    public ConstructionMethodParameterInfo(string name, Property source, Result<GenericFormattableString> result, string collectionInitializer, string suffix)
    {
        Name = name;
        Source = source;
        Result = result;
        CollectionInitializer = collectionInitializer;
        Suffix = suffix;
    }

    public override bool Equals(object? obj)
    {
        return obj is ConstructionMethodParameterInfo other &&
               Name == other.Name &&
               EqualityComparer<Property>.Default.Equals(Source, other.Source) &&
               EqualityComparer<Result<GenericFormattableString>>.Default.Equals(Result, other.Result) &&
               CollectionInitializer == other.CollectionInitializer &&
               Suffix == other.Suffix;
    }

    public override int GetHashCode()
    {
        var hashCode = -673813403;
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
        hashCode = hashCode * -1521134295 + EqualityComparer<Property>.Default.GetHashCode(Source);
        hashCode = hashCode * -1521134295 + EqualityComparer<Result<GenericFormattableString>>.Default.GetHashCode(Result);
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(CollectionInitializer);
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Suffix);
        return hashCode;
    }
}

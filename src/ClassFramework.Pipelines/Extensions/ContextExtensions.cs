namespace ClassFramework.Pipelines.Extensions;

public static class ContextExtensions
{
    public static async Task<Result<GenericFormattableString>> CreateEntityInstanciationAsync(
        this BuilderContext context,
        IExpressionEvaluator evaluator,
        ICsharpExpressionDumper csharpExpressionDumper,
        string classNameSuffix,
        CancellationToken token)
    {
        evaluator = evaluator.IsNotNull(nameof(evaluator));

        var customEntityInstanciation = context
            .GetMappingMetadata(context.SourceModel.GetFullName())
            .GetStringValue(MetadataNames.CustomBuilderEntityInstanciation);
        if (!string.IsNullOrEmpty(customEntityInstanciation))
        {
            return await evaluator.EvaluateInterpolatedStringAsync(customEntityInstanciation, context.FormatProvider, context, token).ConfigureAwait(false);
        }

        if (context.SourceModel is not IConstructorsContainer constructorsContainer)
        {
            return Result.Invalid<GenericFormattableString>("Cannot create an instance of a type that does not have constructors");
        }

        if (context.SourceModel is Class cls && cls.Abstract)
        {
            return Result.Invalid<GenericFormattableString>("Cannot create an instance of an abstract class");
        }

        var hasPublicParameterlessConstructor = constructorsContainer.HasPublicParameterlessConstructor();
        var openSign = GetBuilderPocoOpenSign(hasPublicParameterlessConstructor && context.SourceModel.Properties.Count != 0);
        var closeSign = GetBuilderPocoCloseSign(hasPublicParameterlessConstructor && context.SourceModel.Properties.Count != 0);

        var parametersResult = await GetConstructionMethodParametersAsync(context, evaluator, csharpExpressionDumper, hasPublicParameterlessConstructor, token).ConfigureAwait(false);
        if (!parametersResult.IsSuccessful())
        {
            return parametersResult;
        }

        var entityNamespace = context.GetMappingMetadata(context.SourceModel.GetFullName()).GetStringValue(MetadataNames.CustomEntityNamespace, () => context.SourceModel.Namespace);
        var ns = context.MapNamespace(entityNamespace).AppendWhenNotNullOrEmpty(".");

        return Result.Success<GenericFormattableString>($"new {ns}{context.SourceModel.Name}{classNameSuffix}{context.SourceModel.GetGenericTypeArgumentsString()}{openSign}{parametersResult.Value}{closeSign}");
    }

    public static async Task<string> CreateEntityChainCallAsync(this EntityContext context)
    {
        context = context.IsNotNull(nameof(context));

        return context.Settings.EnableInheritance && context.Settings.BaseClass is not null
            ? $"base({GetPropertyNamesConcatenated(context.Settings.BaseClass.Properties, context.FormatProvider.ToCultureInfo())})"
            : (await context.SourceModel.GetCustomValueForInheritedClassAsync(context.Settings.EnableInheritance,
                cls => Task.FromResult(Result.Success<GenericFormattableString>($"base({GetPropertyNamesConcatenated(context.SourceModel.Properties.Where(x => x.ParentTypeFullName == cls.BaseClass), context.FormatProvider.ToCultureInfo())})"))).ConfigureAwait(false)).Value!; // we can simply shortcut the result evaluation, because we are injecting the Success in the delegate
    }

    public static async Task<Result> ProcessPropertiesAsync<TContext>(
        this TContext context,
        string methodName,
        IEnumerable<Property> properties,
        Func<TContext, Property, CancellationToken, Task<IReadOnlyDictionary<string, Result<GenericFormattableString>>>> getResultsDelegate,
        Func<string, string, string> getReturnTypeDelegate,
        Action<Property, string, IReadOnlyDictionary<string, Result<GenericFormattableString>>, string> processDelegate,
        CancellationToken token)
    {
        context = ArgumentGuard.IsNotNull(context, nameof(context));
        properties = ArgumentGuard.IsNotNull(properties, nameof(properties));
        getResultsDelegate = ArgumentGuard.IsNotNull(getResultsDelegate, nameof(getResultsDelegate));
        getReturnTypeDelegate = ArgumentGuard.IsNotNull(getReturnTypeDelegate, nameof(getReturnTypeDelegate));
        processDelegate = ArgumentGuard.IsNotNull(processDelegate, nameof(processDelegate));

        if (string.IsNullOrEmpty(methodName))
        {
            return Result.Success();
        }

        foreach (var property in properties)
        {
            var results = await getResultsDelegate(context, property, token).ConfigureAwait(false);

            var error = results.GetError();
            if (error is not null)
            {
                // Error in formattable string parsing
                return error;
            }

            var returnType = getReturnTypeDelegate(results.GetValue(ResultNames.Namespace), results.GetValue(ResultNames.BuilderName));

            processDelegate(property, returnType, results, returnType);
        }

        return Result.Success();
    }

    private static async Task<Result<GenericFormattableString>> GetConstructionMethodParametersAsync(BuilderContext context, IExpressionEvaluator evaluator, ICsharpExpressionDumper csharpExpressionDumper, bool hasPublicParameterlessConstructor, CancellationToken token)
    {
        var properties = context.SourceModel.GetBuilderConstructorProperties(context);

        var results = new List<ConstructionMethodParameterInfo>();
        foreach (var property in properties)
        {
            var useBuilderLazyValues = context.UseBuilderLazyValues(property.TypeName);
            var info =
                new ConstructionMethodParameterInfo(
                    property.Name,
                    property,
                    await evaluator.EvaluateInterpolatedStringAsync
                    (
                        context
                            .GetMappingMetadata(property.TypeName)
                            .GetStringValue(MetadataNames.CustomBuilderMethodParameterExpression, PlaceholderNames.NamePlaceholder),
                        context.FormatProvider,
                        new ParentChildContext<BuilderContext, Property>(context, property, context.Settings),
                        token
                    ).ConfigureAwait(false),
                    context.GetMappingMetadata
                    (
                        property.TypeName.FixTypeName().WithoutGenerics() // i.e. List<> etc.
                    ).GetStringValue(MetadataNames.CustomCollectionInitialization, () => "[Expression]"),
                    property.GetSuffix(context.Settings.EnableNullableReferenceTypes, csharpExpressionDumper, context),
                    useBuilderLazyValues
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

        return Result.Success<GenericFormattableString>(string.Join(", ", results.Select(x => $"{GetPrefix(hasPublicParameterlessConstructor, x.Name)}{GetBuilderPropertyExpression(x.Result.Value!, x.Source, x.CollectionInitializer, x.Suffix, x.UseBuilderLazyValues)}")));
    }

    private static string GetPrefix(bool hasPublicParameterlessConstructor, string propertyName)
        => hasPublicParameterlessConstructor
            ? $"{propertyName} = "
            : string.Empty;

    private static string GetPropertyNamesConcatenated(IEnumerable<Property> properties, CultureInfo cultureInfo)
        => string.Join(", ", properties.Select(x => x.Name.ToCamelCase(cultureInfo).GetCsharpFriendlyName()));

    private static string? GetBuilderPropertyExpression(this string? value, Property sourceProperty, string collectionInitializer, string suffix, bool useBuilderLazyValues)
    {
        if (value is null || !value.Contains(PlaceholderNames.NamePlaceholder))
        {
            return value;
        }

        var lazySuffix = GetLazySuffix(sourceProperty, useBuilderLazyValues);

        if (value == PlaceholderNames.NamePlaceholder)
        {
            return sourceProperty.Name + lazySuffix;
        }

        if (sourceProperty.TypeName.FixTypeName().IsCollectionTypeName())
        {
            var valueExpression = value!
                .Replace(PlaceholderNames.NamePlaceholder, "x")
                .Replace("[NullableSuffix]", string.Empty)
                .Replace("[ForcedNullableSuffix]", sourceProperty.IsValueType
                    ? string.Empty
                    : "!");

            return collectionInitializer
                .Replace("[Type]", sourceProperty.TypeName.FixTypeName().WithoutGenerics())
                .Replace("[Generics]", sourceProperty.TypeName.FixTypeName().GetGenericArguments(addBrackets: true))
                .Replace("[Expression]", $"{sourceProperty.Name}{suffix}.Select(x => {valueExpression}{lazySuffix})");
        }
        else
        {
            var valueExpression = value!
                .Replace(PlaceholderNames.NamePlaceholder, sourceProperty.Name)
                .Replace("[NullableSuffix]", suffix)
                .Replace("[ForcedNullableSuffix]", string.IsNullOrEmpty(suffix)
                    ? string.Empty
                    : "!");

            return $"{valueExpression}{lazySuffix}";
        }
    }

    private static string GetLazySuffix(Property sourceProperty, bool useBuilderLazyValues)
    {
        if (!useBuilderLazyValues)
        {
            return string.Empty;
        }

        if (sourceProperty.TypeName.FixTypeName().IsCollectionTypeName())
        {
            // for a collection property, we need to call Invoke using () on each item
            return ".Select(x => x())";
        }

        // for a non collection property, we can just call the Invoke using ()
        return "()";
    }

    private static string GetBuilderPocoCloseSign(bool poco)
        => poco
            ? " }"
            : ")";

    private static string GetBuilderPocoOpenSign(bool poco)
        => poco
            ? " { "
            : "(";
}

internal sealed class ConstructionMethodParameterInfo
{
    public string Name { get; }
    public Property Source { get; }
    public Result<GenericFormattableString> Result { get; }
    public string CollectionInitializer { get; }
    public string Suffix { get; }
    public bool UseBuilderLazyValues { get; }

    public ConstructionMethodParameterInfo(string name, Property source, Result<GenericFormattableString> result, string collectionInitializer, string suffix, bool useBuilderLazyValues)
    {
        Name = name;
        Source = source;
        Result = result;
        CollectionInitializer = collectionInitializer;
        Suffix = suffix;
        UseBuilderLazyValues = useBuilderLazyValues;
    }
}

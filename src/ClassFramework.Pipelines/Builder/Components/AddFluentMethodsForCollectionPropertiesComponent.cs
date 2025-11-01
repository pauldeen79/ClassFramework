namespace ClassFramework.Pipelines.Builder.Components;

public class AddFluentMethodsForCollectionPropertiesComponent(IExpressionEvaluator evaluator) : IPipelineComponent<BuilderContext>
{
    private readonly IExpressionEvaluator _evaluator = evaluator.IsNotNull(nameof(evaluator));

    public async Task<Result> ExecuteAsync(BuilderContext context, ICommandService commandService, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        return await context.ProcessPropertiesAsync(
            context.Settings.AddMethodNameFormatString,
            context.GetSourceProperties().Where(x => context.IsValidForFluentMethod(x) && x.TypeName.FixTypeName().IsCollectionTypeName()),
            GetResultsAsync,
            context.GetReturnTypeForFluentMethod,
            (property, returnType, results, token) => AddMethods(context, property, returnType, results),
            token).ConfigureAwait(false);
    }

    private static void AddMethods(BuilderContext context, Property property, string returnType, IReadOnlyDictionary<string, Result<GenericFormattableString>> results)
    {
        context.Builder.AddMethods(context.GetFluentMethodsForCollectionProperty(property, results, returnType, ResultNames.TypeName, "EnumerableOverload.", "ArrayOverload."));

        if (results.NeedNonLazyOverloads())
        {
            //Add overloads for non-func type
            context.Builder.AddMethods(context.GetFluentMethodsForCollectionProperty(property, results, returnType, ResultNames.NonLazyTypeName, "NonLazyEnumerableOverload.", "NonLazyArrayOverload."));
        }
    }

    private async Task<IReadOnlyDictionary<string, Result<GenericFormattableString>>> GetResultsAsync(BuilderContext context, Property property, CancellationToken token)
    {
        var parentChildContext = new ParentChildContext<BuilderContext, Property>(context, property, context.Settings);

        return await context.GetResultDictionaryForBuilderCollectionProperties(property, parentChildContext, _evaluator)
            .AddRange("EnumerableOverload.{0}", await GetCodeStatementsForEnumerableOverload(context, property, parentChildContext, false, token).ConfigureAwait(false))
            .AddRange("ArrayOverload.{0}", await GetCodeStatementsForArrayOverload(context, property, false, token).ConfigureAwait(false))
            .AddRange("NonLazyArrayOverload.{0}", await GetCodeStatementsForArrayOverload(context, property, true, token).ConfigureAwait(false))
            .AddRange("NonLazyEnumerableOverload.{0}", await GetCodeStatementsForEnumerableOverload(context, property, parentChildContext, true, token).ConfigureAwait(false))
            .Build()
            .ConfigureAwait(false);
    }

    private async Task<IEnumerable<Result<GenericFormattableString>>> GetCodeStatementsForEnumerableOverload(BuilderContext context, Property property, ParentChildContext<BuilderContext, Property> parentChildContext, bool useBuilderLazyValues, CancellationToken token)
    {
        if (context.Settings.BuilderNewCollectionTypeName == typeof(IEnumerable<>).WithoutGenerics())
        {
            // When using IEnumerable<>, do not call ToArray because we want lazy evaluation
            return await GetCodeStatementsForArrayOverload(context, property, useBuilderLazyValues, token).ConfigureAwait(false);
        }

        var results = new List<Result<GenericFormattableString>>();

        // When not using IEnumerable<>, we can simply force ToArray because it's stored in a generic list or collection of some sort anyway.
        // (in other words, materialization is always performed)
        if (context.Settings.AddNullChecks)
        {
            results.Add(Result.Success<GenericFormattableString>(context.CreateArgumentNullException(property.Name.ToCamelCase(context.FormatProvider.ToCultureInfo()).GetCsharpFriendlyName())));
        }

        results.Add(await _evaluator.EvaluateInterpolatedStringAsync("return {addMethodNameFormatString}({CsharpFriendlyName(property.Name.ToCamelCase())}.ToArray());", context.FormatProvider, parentChildContext, token).ConfigureAwait(false));

        return results;
    }

    private async Task<IEnumerable<Result<GenericFormattableString>>> GetCodeStatementsForArrayOverload(BuilderContext context, Property property, bool useBuilderLazyValues, CancellationToken token)
    {
        var results = new List<Result<GenericFormattableString>>();

        if (context.Settings.AddNullChecks)
        {
            var argumentNullCheckResult = await _evaluator.EvaluateInterpolatedStringAsync
            (
                context.GetMappingMetadata(property.TypeName).GetStringValue(MetadataNames.CustomBuilderArgumentNullCheckExpression, "{ArgumentNullCheck()}"),
                context.FormatProvider,
                new ParentChildContext<BuilderContext, Property>(context, property, context.Settings),
                token
            ).ConfigureAwait(false);

            if (!argumentNullCheckResult.IsSuccessful() || !string.IsNullOrEmpty(argumentNullCheckResult.Value?.ToString()))
            {
                results.Add(argumentNullCheckResult);
            }
        }

        var builderAddExpressionResult = await _evaluator.EvaluateInterpolatedStringAsync
        (
            context
                .GetMappingMetadata(property.TypeName)
                .GetStringValue(MetadataNames.CustomBuilderAddExpression, useBuilderLazyValues
                    ? context.Settings.NonLazyCollectionCopyStatementFormatString
                    : context.Settings.CollectionCopyStatementFormatString),
            context.FormatProvider,
            new ParentChildContext<BuilderContext, Property>(context, property, context.Settings),
            token
        ).ConfigureAwait(false);

        results.Add(builderAddExpressionResult);
        results.Add(Result.Success<GenericFormattableString>(context.ReturnValueStatementForFluentMethod));

        return results;
    }
}

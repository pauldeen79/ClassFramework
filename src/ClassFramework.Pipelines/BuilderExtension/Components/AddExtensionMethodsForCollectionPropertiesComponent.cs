namespace ClassFramework.Pipelines.BuilderExtension.Components;

public class AddExtensionMethodsForCollectionPropertiesComponent(IExpressionEvaluator evaluator) : IPipelineComponent<BuilderExtensionContext>
{
    private readonly IExpressionEvaluator _evaluator = evaluator.IsNotNull(nameof(evaluator));

    public async Task<Result> ProcessAsync(PipelineContext<BuilderExtensionContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        return await context.ProcessPropertiesAsync(
            context.Request.Settings.AddMethodNameFormatString,
            context.Request.GetSourceProperties().Where(x => x.TypeName.FixTypeName().IsCollectionTypeName()),
            GetResultsAsync,
            context.Request.GetReturnTypeForFluentMethod,
            (property, returnType, results, token) => AddMethods(context, property, returnType, results),
            token).ConfigureAwait(false);
    }

    private static void AddMethods(PipelineContext<BuilderExtensionContext> context, Property property, string returnType, IReadOnlyDictionary<string, Result<GenericFormattableString>> results)
    {
        context.Request.Builder.AddMethods(context.Request.GetFluentMethodsForCollectionProperty(property, results, returnType, ResultNames.TypeName, "EnumerableOverload.", "ArrayOverload."));

        if (results.NeedNonLazyOverloads())
        {
            //Add overloads for non-func type
            context.Request.Builder.AddMethods(context.Request.GetFluentMethodsForCollectionProperty(property, results, returnType, ResultNames.NonLazyTypeName, "NonLazyEnumerableOverload.", "NonLazyArrayOverload."));
        }
    }

    private async Task<IReadOnlyDictionary<string, Result<GenericFormattableString>>> GetResultsAsync(PipelineContext<BuilderExtensionContext> context, Property property, CancellationToken token)
    {
        var parentChildContext = new ParentChildContext<PipelineContext<BuilderExtensionContext>, Property>(context, property, context.Request.Settings);

        return await context.Request.GetResultDictionaryForBuilderCollectionProperties(property, parentChildContext, _evaluator)
            .AddRange("EnumerableOverload.{0}", await GetCodeStatementsForEnumerableOverload(context, property, parentChildContext, token).ConfigureAwait(false))
            .AddRange("ArrayOverload.{0}", await GetCodeStatementsForArrayOverload(context, property, false, token).ConfigureAwait(false))
            .AddRange("NonLazyArrayOverload.{0}", await GetCodeStatementsForArrayOverload(context, property, true, token).ConfigureAwait(false))
            .Build()
            .ConfigureAwait(false);
    }

    private async Task<IEnumerable<Result<GenericFormattableString>>> GetCodeStatementsForEnumerableOverload(PipelineContext<BuilderExtensionContext> context, Property property, ParentChildContext<PipelineContext<BuilderExtensionContext>, Property> parentChildContext, CancellationToken token)
    {
        var results = new List<Result<GenericFormattableString>>();

        if (context.Request.Settings.AddNullChecks)
        {
            results.Add(Result.Success<GenericFormattableString>(context.Request.CreateArgumentNullException(property.Name.ToCamelCase(context.Request.FormatProvider.ToCultureInfo()).GetCsharpFriendlyName())));
        }

        results.Add(await _evaluator.EvaluateInterpolatedStringAsync("return instance.{addMethodNameFormatString}<T>({CsharpFriendlyName(property.Name.ToCamelCase())}.ToArray());", context.Request.FormatProvider, parentChildContext, token).ConfigureAwait(false));

        return results;
    }

    private async Task<IEnumerable<Result<GenericFormattableString>>> GetCodeStatementsForArrayOverload(PipelineContext<BuilderExtensionContext> context, Property property, bool useBuilderLazyValues, CancellationToken token)
    {
        var results = new List<Result<GenericFormattableString>>();

        if (context.Request.Settings.AddNullChecks)
        {
            var argumentNullCheckResult = await _evaluator.EvaluateInterpolatedStringAsync
            (
                context.Request.GetMappingMetadata(property.TypeName).GetStringValue(MetadataNames.CustomBuilderArgumentNullCheckExpression, "{ArgumentNullCheck()}"),
                context.Request.FormatProvider,
                new ParentChildContext<PipelineContext<BuilderExtensionContext>, Property>(context, property, context.Request.Settings),
                token
            ).ConfigureAwait(false);

            results.Add(argumentNullCheckResult);
        }

        var builderAddExpressionResult = await _evaluator.EvaluateInterpolatedStringAsync
        (
            context.Request
                .GetMappingMetadata(property.TypeName)
                .GetStringValue(MetadataNames.CustomBuilderAddExpression, useBuilderLazyValues
                    ? context.Request.Settings.NonLazyBuilderExtensionsCollectionCopyStatementFormatString
                    : context.Request.Settings.BuilderExtensionsCollectionCopyStatementFormatString),
            context.Request.FormatProvider,
            new ParentChildContext<PipelineContext<BuilderExtensionContext>, Property>(context, property, context.Request.Settings),
            token
        ).ConfigureAwait(false);

        results.Add(builderAddExpressionResult);
        results.Add(Result.Success<GenericFormattableString>("return instance;"));

        return results;
    }
}

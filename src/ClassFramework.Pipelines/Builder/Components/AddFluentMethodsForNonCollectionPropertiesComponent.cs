namespace ClassFramework.Pipelines.Builder.Components;

public class AddFluentMethodsForNonCollectionPropertiesComponent(IExpressionEvaluator evaluator) : IPipelineComponent<BuilderContext>
{
    private readonly IExpressionEvaluator _evaluator = evaluator.IsNotNull(nameof(evaluator));

    public async Task<Result> ProcessAsync(PipelineContext<BuilderContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        return await context.ProcessPropertiesAsync(
            context.Request.Settings.SetMethodNameFormatString,
            context.Request.GetSourceProperties().Where(x => context.Request.IsValidForFluentMethod(x) && !x.TypeName.FixTypeName().IsCollectionTypeName()),
            GetResultsAsync,
            context.Request.GetReturnTypeForFluentMethod,
            (property, returnType, results, token) => AddMethods(context, property, returnType, results),
            token).ConfigureAwait(false);
    }

    private static void AddMethods(PipelineContext<BuilderContext> context, Property property, string returnType, IReadOnlyDictionary<string, Result<GenericFormattableString>> results)
    {
        context.Request.Builder.AddMethods(context.Request.GetFluentMethodsForNonCollectionProperty(property, results, returnType, ResultNames.TypeName, ResultNames.BuilderWithExpression));

        if (results.NeedNonLazyOverloads())
        {
            //Add overload for non-func type
            context.Request.Builder.AddMethods(context.Request.GetFluentMethodsForNonCollectionProperty(property, results, returnType, ResultNames.NonLazyTypeName, ResultNames.BuilderNonLazyWithExpression));
        }
    }

    private Task<IReadOnlyDictionary<string, Result<GenericFormattableString>>> GetResultsAsync(PipelineContext<BuilderContext> context, Property property, CancellationToken token)
        => context.Request.GetResultsForBuilderNonCollectionProperties(
            property,
            new ParentChildContext<PipelineContext<BuilderContext>, Property>(context, property, context.Request.Settings),
            _evaluator);
}

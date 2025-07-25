namespace ClassFramework.Pipelines.BuilderExtension.Components;

public class AddExtensionMethodsForNonCollectionPropertiesComponent(IExpressionEvaluator evaluator) : IPipelineComponent<BuilderExtensionContext>
{
    private readonly IExpressionEvaluator _evaluator = evaluator.IsNotNull(nameof(evaluator));

    public async Task<Result> ProcessAsync(PipelineContext<BuilderExtensionContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        if (string.IsNullOrEmpty(context.Request.Settings.SetMethodNameFormatString))
        {
            return Result.Success();
        }

        foreach (var property in context.Request.GetSourceProperties()
            .Where(x => !x.TypeName.FixTypeName().IsCollectionTypeName()))
        {
            var parentChildContext = new ParentChildContext<PipelineContext<BuilderExtensionContext>, Property>(context, property, context.Request.Settings);

            var results = await context.Request.GetResultsForBuilderNonCollectionProperties(property, parentChildContext, _evaluator).ConfigureAwait(false);

            var error = results.GetError();
            if (error is not null)
            {
                // Error in formattable string parsing
                return error;
            }

            var returnType = context.Request.GetReturnTypeForFluentMethod(results.GetValue(ResultNames.Namespace), results.GetValue(ResultNames.BuilderName));

            context.Request.Builder.AddMethods(context.Request.GetFluentMethodsForNonCollectionProperty(property, results, returnType, ResultNames.TypeName, ResultNames.BuilderWithExpression));

            if (results.NeedNonLazyOverloads())
            {
                //Add overload for non-func type
                context.Request.Builder.AddMethods(context.Request.GetFluentMethodsForNonCollectionProperty(property, results, returnType, ResultNames.NonLazyTypeName, ResultNames.BuilderNonLazyWithExpression));
            }
        }

        return Result.Success();
    }
}

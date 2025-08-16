namespace ClassFramework.Pipelines.BuilderExtension.Components;

public class SetNameComponent(IExpressionEvaluator evaluator) : IPipelineComponent<BuilderExtensionContext>
{
    private readonly IExpressionEvaluator _evaluator = evaluator.IsNotNull(nameof(evaluator));

    public async Task<Result> ProcessAsync(PipelineContext<BuilderExtensionContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        return (await new AsyncResultDictionaryBuilder<GenericFormattableString>()
            .Add(ResultNames.Name, _evaluator.EvaluateInterpolatedStringAsync(context.Request.Settings.BuilderExtensionsNameFormatString, context.Request.FormatProvider, context.Request, token))
            .Add(ResultNames.Namespace, _evaluator.EvaluateInterpolatedStringAsync(context.Request.Settings.BuilderExtensionsNamespaceFormatString, context.Request.FormatProvider, context.Request, token))
            .Build()
            .ConfigureAwait(false))
            .OnSuccess(results =>
            {
                context.Request.Builder
                    .WithName(results.GetValue(ResultNames.Name))
                    .WithNamespace(results.GetValue(ResultNames.Namespace));
            });
    }
}

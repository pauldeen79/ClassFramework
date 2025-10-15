namespace ClassFramework.Pipelines.Builder.Components;

public class SetNameComponent(IExpressionEvaluator evaluator) : IPipelineComponent<BuilderContext>, IOrderContainer
{
    private readonly IExpressionEvaluator _evaluator = evaluator.IsNotNull(nameof(evaluator));

    public int Order => PipelineStage.Process;

    public async Task<Result> ProcessAsync(PipelineContext<BuilderContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        return (await new AsyncResultDictionaryBuilder<GenericFormattableString>()
            .Add(ResultNames.Name, _evaluator.EvaluateInterpolatedStringAsync(context.Request.Settings.BuilderNameFormatString, context.Request.FormatProvider, context.Request, token))
            .Add(ResultNames.Namespace, context.Request.GetMappingMetadata(context.Request.SourceModel.GetFullName()).GetGenericFormattableStringAsync(MetadataNames.CustomBuilderNamespace, _evaluator.EvaluateInterpolatedStringAsync(context.Request.Settings.BuilderNamespaceFormatString, context.Request.FormatProvider, context.Request, token)))
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

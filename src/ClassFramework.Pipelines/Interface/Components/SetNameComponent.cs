namespace ClassFramework.Pipelines.Interface.Components;

public class SetNameComponent(IExpressionEvaluator evaluator) : IPipelineComponent<InterfaceContext>, IOrderContainer
{
    private readonly IExpressionEvaluator _evaluator = evaluator.IsNotNull(nameof(evaluator));

    public int Order => PipelineStage.Process;

    public async Task<Result> ProcessAsync(PipelineContext<InterfaceContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        return (await new AsyncResultDictionaryBuilder<GenericFormattableString>()
            .Add(ResultNames.Name, () => _evaluator.EvaluateInterpolatedStringAsync(context.Request.Settings.NameFormatString, context.Request.FormatProvider, context.Request, token))
            .Add(ResultNames.Namespace, () => context.Request.GetMappingMetadata(context.Request.SourceModel.GetFullName()).GetGenericFormattableStringAsync(MetadataNames.CustomEntityNamespace, _evaluator.EvaluateInterpolatedStringAsync(context.Request.Settings.NamespaceFormatString, context.Request.FormatProvider, context.Request, token)))
            .Build()
            .ConfigureAwait(false))
            .OnSuccess(results =>
            {
                context.Request.Builder
                    .WithName(results.GetValue(ResultNames.Name))
                    .WithNamespace(context.Request.MapNamespace(results.GetValue(ResultNames.Namespace)));
            });
    }
}

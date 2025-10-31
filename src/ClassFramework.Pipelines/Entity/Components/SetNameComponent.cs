namespace ClassFramework.Pipelines.Entity.Components;

public class SetNameComponent(IExpressionEvaluator evaluator) : IPipelineComponent<EntityContext>, IOrderContainer
{
    private readonly IExpressionEvaluator _evaluator = evaluator.IsNotNull(nameof(evaluator));

    public int Order => PipelineStage.Process;

    public async Task<Result> ExecuteAsync(EntityContext context, ICommandService commandService, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        return (await new AsyncResultDictionaryBuilder<GenericFormattableString>()
            .Add(ResultNames.Name, () => _evaluator.EvaluateInterpolatedStringAsync(context.Settings.EntityNameFormatString, context.FormatProvider, context, token))
            .Add(ResultNames.Namespace, () => context.GetMappingMetadata(context.SourceModel.GetFullName()).GetGenericFormattableStringAsync(MetadataNames.CustomEntityNamespace, _evaluator.EvaluateInterpolatedStringAsync(context.Settings.EntityNamespaceFormatString, context.FormatProvider, context, token)))
            .Build()
            .ConfigureAwait(false))
            .OnSuccess(results =>
            {
                context.Builder
                    .WithName(results.GetValue(ResultNames.Name))
                    .WithNamespace(context.MapNamespace(results.GetValue(ResultNames.Namespace)));

            });
    }
}

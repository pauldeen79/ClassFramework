namespace ClassFramework.Pipelines.Interface.Components;

public class SetNameComponent(IExpressionEvaluator evaluator) : IPipelineComponent<InterfaceContext, InterfaceBuilder>
{
    private readonly IExpressionEvaluator _evaluator = evaluator.IsNotNull(nameof(evaluator));

    public async Task<Result> ExecuteAsync(InterfaceContext context, InterfaceBuilder response, ICommandService commandService, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        return (await new AsyncResultDictionaryBuilder<GenericFormattableString>()
            .Add(ResultNames.Name, () => _evaluator.EvaluateInterpolatedStringAsync(context.Settings.NameFormatString, context.FormatProvider, context, token))
            .Add(ResultNames.Namespace, () => context.GetMappingMetadata(context.SourceModel.GetFullName()).GetGenericFormattableStringAsync(MetadataNames.CustomEntityNamespace, _evaluator.EvaluateInterpolatedStringAsync(context.Settings.NamespaceFormatString, context.FormatProvider, context, token)))
            .Build()
            .ConfigureAwait(false))
            .OnSuccess(results =>
            {
                response
                    .WithName(results.GetValue(ResultNames.Name))
                    .WithNamespace(context.MapNamespace(results.GetValue(ResultNames.Namespace)));
            });
    }
}

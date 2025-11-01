namespace ClassFramework.Pipelines.Reflection.Components;

public class SetNameComponent(IExpressionEvaluator evaluator) : IPipelineComponent<ReflectionContext>
{
    private readonly IExpressionEvaluator _evaluator = evaluator.IsNotNull(nameof(evaluator));

    public async Task<Result> ExecuteAsync(ReflectionContext context, ICommandService commandService, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        return (await new AsyncResultDictionaryBuilder<GenericFormattableString>()
            .Add(ResultNames.Name, () => _evaluator.EvaluateInterpolatedStringAsync(context.Settings.NameFormatString, context.FormatProvider, context, token))
            .Add(ResultNames.Namespace, () => _evaluator.EvaluateInterpolatedStringAsync(context.Settings.NamespaceFormatString, context.FormatProvider, context, token))
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

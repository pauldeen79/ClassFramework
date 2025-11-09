namespace ClassFramework.Pipelines.Reflection.Components;

public class SetNameComponent(IExpressionEvaluator evaluator) : IPipelineComponent<GenerateTypeFromReflectionCommand, TypeBaseBuilder>
{
    private readonly IExpressionEvaluator _evaluator = evaluator.IsNotNull(nameof(evaluator));

    public async Task<Result> ExecuteAsync(GenerateTypeFromReflectionCommand context, TypeBaseBuilder response, ICommandService commandService, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));
        response = response.IsNotNull(nameof(response));

        return (await new AsyncResultDictionaryBuilder<GenericFormattableString>()
            .Add(ResultNames.Name, () => _evaluator.EvaluateInterpolatedStringAsync(context.Settings.NameFormatString, context.FormatProvider, context, token))
            .Add(ResultNames.Namespace, () => _evaluator.EvaluateInterpolatedStringAsync(context.Settings.NamespaceFormatString, context.FormatProvider, context, token))
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

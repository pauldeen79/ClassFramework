namespace ClassFramework.Pipelines.Reflection.Components;

public class SetNameComponent(IExpressionEvaluator evaluator) : IPipelineComponent<GenerateTypeFromReflectionCommand, TypeBaseBuilder>
{
    private readonly IExpressionEvaluator _evaluator = evaluator.IsNotNull(nameof(evaluator));

    public async Task<Result> ExecuteAsync(GenerateTypeFromReflectionCommand command, TypeBaseBuilder response, ICommandService commandService, CancellationToken token)
    {
        command = command.IsNotNull(nameof(command));
        response = response.IsNotNull(nameof(response));

        return (await new AsyncResultDictionaryBuilder<GenericFormattableString>()
            .Add(ResultNames.Name, () => _evaluator.EvaluateInterpolatedStringAsync(command.Settings.NameFormatString, command.FormatProvider, command, token))
            .Add(ResultNames.Namespace, () => _evaluator.EvaluateInterpolatedStringAsync(command.Settings.NamespaceFormatString, command.FormatProvider, command, token))
            .BuildAsync(token)
            .ConfigureAwait(false))
            .OnSuccess(results =>
            {
                response
                    .WithName(results.GetValue(ResultNames.Name))
                    .WithNamespace(command.MapNamespace(results.GetValue(ResultNames.Namespace)));
            });
    }
}

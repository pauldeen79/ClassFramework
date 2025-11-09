namespace ClassFramework.Pipelines.BuilderExtension.Components;

public class SetNameComponent(IExpressionEvaluator evaluator) : IPipelineComponent<GenerateBuilderExtensionCommand, ClassBuilder>
{
    private readonly IExpressionEvaluator _evaluator = evaluator.IsNotNull(nameof(evaluator));

    public async Task<Result> ExecuteAsync(GenerateBuilderExtensionCommand command, ClassBuilder response, ICommandService commandService, CancellationToken token)
    {
        command = command.IsNotNull(nameof(command));
        response = response.IsNotNull(nameof(response));

        return (await new AsyncResultDictionaryBuilder<GenericFormattableString>()
            .Add(ResultNames.Name, () => _evaluator.EvaluateInterpolatedStringAsync(command.Settings.BuilderExtensionsNameFormatString, command.FormatProvider, command, token))
            .Add(ResultNames.Namespace, () => _evaluator.EvaluateInterpolatedStringAsync(command.Settings.BuilderExtensionsNamespaceFormatString, command.FormatProvider, command, token))
            .Build()
            .ConfigureAwait(false))
            .OnSuccess(results =>
            {
                response
                    .WithName(results.GetValue(ResultNames.Name))
                    .WithNamespace(results.GetValue(ResultNames.Namespace));
            });
    }
}

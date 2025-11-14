namespace ClassFramework.Pipelines.Builder.Components;

public class SetNameComponent(IExpressionEvaluator evaluator) : IPipelineComponent<GenerateBuilderCommand, ClassBuilder>
{
    private readonly IExpressionEvaluator _evaluator = evaluator.IsNotNull(nameof(evaluator));

    public async Task<Result> ExecuteAsync(GenerateBuilderCommand command, ClassBuilder response, ICommandService commandService, CancellationToken token)
    {
        command = command.IsNotNull(nameof(command));
        response = response.IsNotNull(nameof(response));

        return (await new AsyncResultDictionaryBuilder<GenericFormattableString>()
            .Add(ResultNames.Name, () => _evaluator.EvaluateInterpolatedStringAsync(command.Settings.BuilderNameFormatString, command.FormatProvider, command, token))
            .Add(ResultNames.Namespace, () => command.GetMappingMetadata(command.SourceModel.GetFullName()).GetGenericFormattableStringAsync(MetadataNames.CustomBuilderNamespace, _evaluator.EvaluateInterpolatedStringAsync(command.Settings.BuilderNamespaceFormatString, command.FormatProvider, command, token)))
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

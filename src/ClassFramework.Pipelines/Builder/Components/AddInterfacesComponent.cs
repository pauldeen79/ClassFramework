namespace ClassFramework.Pipelines.Builder.Components;

public class AddInterfacesComponent(IExpressionEvaluator evaluator) : IPipelineComponent<GenerateBuilderCommand, ClassBuilder>
{
    private readonly IExpressionEvaluator _evaluator = evaluator.IsNotNull(nameof(evaluator));

    public async Task<Result> ExecuteAsync(GenerateBuilderCommand command, ClassBuilder response, ICommandService commandService, CancellationToken token)
    {
        command = command.IsNotNull(nameof(command));
        response = response.IsNotNull(nameof(response));

        if (!command.Settings.CopyInterfaces)
        {
            return Result.Continue();
        }

        var interfaces = await command.GetInterfaceResultsAsync(
            (_, x) => x.ToString(),
            x => command.MapTypeName(x.FixTypeName()),
            _evaluator,
            true,
            token).ConfigureAwait(false);

        var error = Array.Find(interfaces, x => !x.IsSuccessful());
        if (error is not null)
        {
            return error;
        }

        response.AddInterfaces(interfaces.Select(x => x.Value!));

        return Result.Success();
    }
}

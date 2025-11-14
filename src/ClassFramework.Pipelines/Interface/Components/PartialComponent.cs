namespace ClassFramework.Pipelines.Interface.Components;

public class PartialComponent : IPipelineComponent<GenerateInterfaceCommand, InterfaceBuilder>
{
    public Task<Result> ExecuteAsync(GenerateInterfaceCommand command, InterfaceBuilder response, ICommandService commandService, CancellationToken token)
        => Task.Run(() =>
        {
            command = command.IsNotNull(nameof(command));
            response = response.IsNotNull(nameof(response));

            response.WithPartial(command.Settings.CreateAsPartial);

            return Result.Success();
        }, token);
}

namespace ClassFramework.Pipelines.Interface.Components;

public class AddInterfacesComponent : IPipelineComponent<GenerateInterfaceCommand, InterfaceBuilder>
{
    public Task<Result> ExecuteAsync(GenerateInterfaceCommand command, InterfaceBuilder response, ICommandService commandService, CancellationToken token)
        => Task.Run(() =>
        {
            command = command.IsNotNull(nameof(command));
            response = response.IsNotNull(nameof(response));

            if (!command.Settings.CopyInterfaces)
            {
                return Result.Continue();
            }

            response.AddInterfaces(command.SourceModel.Interfaces
                .Where(x => command.Settings.CopyInterfacePredicate?.Invoke(x) ?? true)
                .Select(x => command.MapTypeName(x.FixTypeName()))
                .Where(x => !string.IsNullOrEmpty(x)));

            return Result.Success();
        }, token);
}

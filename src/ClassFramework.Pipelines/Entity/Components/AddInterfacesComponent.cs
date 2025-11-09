namespace ClassFramework.Pipelines.Entity.Components;

public class AddInterfacesComponent : IPipelineComponent<GenerateEntityCommand, ClassBuilder>
{
    public async Task<Result> ExecuteAsync(GenerateEntityCommand command, ClassBuilder response, ICommandService commandService, CancellationToken token)
    {
        command = command.IsNotNull(nameof(command));
        response = response.IsNotNull(nameof(response));

        if (!command.Settings.CopyInterfaces)
        {
            return Result.Continue();
        }

        var baseClass = await command.SourceModel.GetEntityBaseClassAsync(command.Settings.EnableInheritance, command.Settings.BaseClass).ConfigureAwait(false);

        response.AddInterfaces(command.SourceModel.Interfaces
            .Where(x => command.Settings.CopyInterfacePredicate?.Invoke(x) ?? true)
            .Where(x => x != baseClass)
            .Select(x => command.MapTypeName(x.FixTypeName()))
            .Where(x => !string.IsNullOrEmpty(x)));

        return Result.Success();
    }
}

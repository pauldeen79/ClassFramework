namespace ClassFramework.Pipelines.Entity.Components;

public class SetBaseClassComponent : IPipelineComponent<GenerateEntityCommand, ClassBuilder>
{
    public async Task<Result> ExecuteAsync(GenerateEntityCommand command, ClassBuilder response, ICommandService commandService, CancellationToken token)
    {
        command = command.IsNotNull(nameof(command));
        response = response.IsNotNull(nameof(response));

        response.WithBaseClass(await command.SourceModel.GetEntityBaseClassAsync(command.Settings.EnableInheritance, command.Settings.BaseClass).ConfigureAwait(false));

        return Result.Success();
    }
}

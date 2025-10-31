namespace ClassFramework.Pipelines.BuilderExtension;

public class BuilderExtensionCommandHandler : ICommandHandler<BuilderExtensionContext, ClassBuilder>
{
    public async Task<Result<ClassBuilder>> ExecuteAsync(BuilderExtensionContext command, ICommandService commandService, CancellationToken token)
    {
        command = ArgumentGuard.IsNotNull(command, nameof(command));
        commandService = ArgumentGuard.IsNotNull(commandService, nameof(commandService));

        return (await commandService.ExecuteAsync(command, token).ConfigureAwait(false))
            .OnSuccess(_ => Result.Success(command.Builder));
    }
}

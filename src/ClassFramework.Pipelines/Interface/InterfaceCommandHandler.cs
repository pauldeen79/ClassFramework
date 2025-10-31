namespace ClassFramework.Pipelines.Interface;

public class InterfaceCommandHandler : ICommandHandler<InterfaceContext, InterfaceBuilder>
{
    public async Task<Result<InterfaceBuilder>> ExecuteAsync(InterfaceContext command, ICommandService commandService, CancellationToken token)
    {
        command = ArgumentGuard.IsNotNull(command, nameof(command));
        commandService = ArgumentGuard.IsNotNull(commandService, nameof(commandService));

        return (await commandService.ExecuteAsync(command, token).ConfigureAwait(false))
            .OnSuccess(_ => Result.Success(command.Builder));
    }
}

namespace ClassFramework.Pipelines.CommandHandlers;

public class ContextCommandHandler<TContext, TBuilder> : ICommandHandler<TContext, TBuilder>
    where TContext : ContextBase
    where TBuilder : TypeBaseBuilder
{
    public async Task<Result<TBuilder>> ExecuteAsync(TContext command, ICommandService commandService, CancellationToken token)
    {
        command = ArgumentGuard.IsNotNull(command, nameof(command));
        commandService = ArgumentGuard.IsNotNull(commandService, nameof(commandService));

        return (await commandService.ExecuteAsync(command, token).ConfigureAwait(false))
            .OnSuccess(_ => Result.Success(command.GetResponseBuilder()).TryCastAllowNull<TBuilder>());
    }
}

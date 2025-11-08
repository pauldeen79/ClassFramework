namespace ClassFramework.Pipelines.CommandHandlers;

public class ContextCommandHandler<TContext, TEntity> : ICommandHandler<TContext, TEntity>
    where TContext : ContextBase
    where TEntity : TypeBase
{
    public async Task<Result<TEntity>> ExecuteAsync(TContext command, ICommandService commandService, CancellationToken token)
    {
        command = ArgumentGuard.IsNotNull(command, nameof(command));
        commandService = ArgumentGuard.IsNotNull(commandService, nameof(commandService));

        return (await commandService.ExecuteAsync(command, token).ConfigureAwait(false))
            .OnSuccess(_ => Result.Success(command.GetResponseEntity()).TryCast<TEntity>());
    }
}

public class ContextCommandHandler<TContext, TBuilder, TEntity> : ICommandHandler<TContext, TEntity>
    where TContext : ContextBase
    where TBuilder : IBuilder<TEntity>
    where TEntity : TypeBase
{
    public async Task<Result<TEntity>> ExecuteAsync(TContext command, ICommandService commandService, CancellationToken token)
    {
        command = ArgumentGuard.IsNotNull(command, nameof(command));
        commandService = ArgumentGuard.IsNotNull(commandService, nameof(commandService));

        return (await commandService.ExecuteAsync<TContext, TBuilder>(command, token).ConfigureAwait(false))
            .OnSuccess(result => result.Build());
    }
}

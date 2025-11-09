namespace ClassFramework.Pipelines.CommandHandlers;

public class ClassFrameworkCommandHandler<TContext, TEntity> : ICommandHandler<TContext, TEntity>
    where TContext : CommandBase
    where TEntity : TypeBase
{
    public async Task<Result<TEntity>> ExecuteAsync(TContext command, ICommandService commandService, CancellationToken token)
    {
        command = ArgumentGuard.IsNotNull(command, nameof(command));
        commandService = ArgumentGuard.IsNotNull(commandService, nameof(commandService));

        return (await command.ExecuteCommandAsync(commandService, command, token).ConfigureAwait(false))
            .OnSuccess(result => (TEntity)(object)result.Build());
    }
}

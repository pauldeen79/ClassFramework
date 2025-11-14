namespace ClassFramework.Pipelines.CommandHandlers;

public class ClassFrameworkCommandHandler<TCommand, TEntity> : ICommandHandler<TCommand, TEntity>
    where TCommand : CommandBase
    where TEntity : TypeBase
{
    public async Task<Result<TEntity>> ExecuteAsync(TCommand command, ICommandService commandService, CancellationToken token)
    {
        command = ArgumentGuard.IsNotNull(command, nameof(command));

        return (await command.ExecuteCommandAsync(commandService, command, token).ConfigureAwait(false))
            .OnSuccess(result => (TEntity)(object)result.Build());
    }
}

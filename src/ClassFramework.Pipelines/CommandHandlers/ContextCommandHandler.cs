namespace ClassFramework.Pipelines.CommandHandlers;

public class ContextCommandHandler<TContext, TEntity> : ICommandHandler<TContext, TEntity>
    where TContext : ContextBase
    where TEntity : TypeBase
{
    public async Task<Result<TEntity>> ExecuteAsync(TContext command, ICommandService commandService, CancellationToken token)
    {
        command = ArgumentGuard.IsNotNull(command, nameof(command));
        commandService = ArgumentGuard.IsNotNull(commandService, nameof(commandService));

        //TODO: Refactor this code, so you don't violate the open/closed principle. Maybe add annotation or some abstract member on ContextBase, so you can just get this builder type directly.
        if (typeof(TContext) == typeof(BuilderContext)
            || typeof(TContext) == typeof(BuilderExtensionContext)
            || typeof(TContext) == typeof(EntityContext))
        {
            return (await commandService.ExecuteAsync<TContext, ClassBuilder>(command, token).ConfigureAwait(false)).OnSuccess(result => (TEntity)(object)result.Build());
        }

        if (typeof(TContext) == typeof(Reflection.ReflectionContext))
        {
            return (await commandService.ExecuteAsync<TContext, TypeBaseBuilder>(command, token).ConfigureAwait(false)).OnSuccess(result => (TEntity)(object)result.Build());
        }

        if (typeof(TContext) == typeof(InterfaceContext))
        {
            return (await commandService.ExecuteAsync<TContext, InterfaceBuilder>(command, token).ConfigureAwait(false)).OnSuccess(result => (TEntity)(object)result.Build());
        }

        return Result.NotSupported<TEntity>($"Unsupported context: {typeof(TContext).FullName}");
    }
}

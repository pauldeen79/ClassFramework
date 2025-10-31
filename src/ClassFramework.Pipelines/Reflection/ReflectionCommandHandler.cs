namespace ClassFramework.Pipelines.Reflection;

public class ReflectionCommandHandler : ICommandHandler<ReflectionContext, TypeBaseBuilder>
{
    public async Task<Result<TypeBaseBuilder>> ExecuteAsync(ReflectionContext command, ICommandService commandService, CancellationToken token)
    {
        command = ArgumentGuard.IsNotNull(command, nameof(command));
        commandService = ArgumentGuard.IsNotNull(commandService, nameof(commandService));

        return (await commandService.ExecuteAsync(command, token).ConfigureAwait(false))
            .OnSuccess(_ => Result.Success(command.Builder));
    }
}

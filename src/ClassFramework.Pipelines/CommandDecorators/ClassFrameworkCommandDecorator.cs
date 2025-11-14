namespace ClassFramework.Pipelines.CommandDecorators;

public class ClassFrameworkCommandDecorator : ICommandDecorator
{
    private readonly ICommandDecorator _decoratee;

    public ClassFrameworkCommandDecorator(ICommandDecorator decoratee)
    {
        ArgumentGuard.IsNotNull(decoratee, nameof(decoratee));

        _decoratee = decoratee;
    }

    public Task<Result> ExecuteAsync<TCommand>(ICommandHandler<TCommand> handler, TCommand command, ICommandService commandService, CancellationToken token)
        => _decoratee.ExecuteAsync(handler, command, commandService, token);

    public async Task<Result<TResponse>> ExecuteAsync<TCommand, TResponse>(ICommandHandler<TCommand, TResponse> handler, TCommand command, ICommandService commandService, CancellationToken token)
    {
        if (command is CommandBase commandBase)
        {
            return await ExecuteClassFrameworkCommand(handler, command, commandService, commandBase, token).ConfigureAwait(false);
        }

        return await _decoratee.ExecuteAsync(handler, command, commandService, token).ConfigureAwait(false);
    }

    private async Task<Result<TResponse>> ExecuteClassFrameworkCommand<TCommand, TResponse>(ICommandHandler<TCommand, TResponse> handler, TCommand command, ICommandService commandService, CommandBase commandBase, CancellationToken token)
    {
        if (!commandBase.Settings.AllowGenerationWithoutProperties
            && !commandBase.Settings.EnableInheritance
            && commandBase.SourceModelHasNoProperties())
        {
            return Result.Invalid<TResponse>("There must be at least one property");
        }

        return (await _decoratee.ExecuteAsync(handler, command, commandService, token).ConfigureAwait(false))
            .OnSuccess(result =>
            {
                var validationResults = new List<ValidationResult>();
                if (result.Value is not null && !result.Value.TryValidate(validationResults))
                {
                    return Result.Invalid<TResponse>(validationResults.Select(x => new ValidationError(x.ErrorMessage, x.MemberNames)));
                }

                return result;
            });
    }
}

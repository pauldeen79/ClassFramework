namespace ClassFramework.Pipelines.CommandDecorators;

public class ValidateCommandDecorator : ICommandDecorator
{
    private readonly ICommandDecorator _decoratee;

    public ValidateCommandDecorator(ICommandDecorator decoratee)
    {
        ArgumentGuard.IsNotNull(decoratee, nameof(decoratee));

        _decoratee = decoratee;
    }

    public Task<Result> ExecuteAsync<TCommand>(ICommandHandler<TCommand> handler, TCommand command, ICommandService commandService, CancellationToken token)
        => _decoratee.ExecuteAsync(handler, command, commandService, token);

    public async Task<Result<TResponse>> ExecuteAsync<TCommand, TResponse>(ICommandHandler<TCommand, TResponse> handler, TCommand command, ICommandService commandService, CancellationToken token)
    {
        if (command is ContextBase context
            && !context.Settings.AllowGenerationWithoutProperties
            && !context.Settings.EnableInheritance
            && context.SourceModelHasNoProperties())
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

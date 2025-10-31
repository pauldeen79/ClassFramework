namespace ClassFramework.Pipelines.CommandDecorators;

public class ValidateCommandDecorator : ICommandDecorator
{
    private readonly ICommandDecorator _decoratee;

    public ValidateCommandDecorator(ICommandDecorator decoratee)
    {
        ArgumentGuard.IsNotNull(decoratee, nameof(decoratee));

        _decoratee = decoratee;
    }

    public async Task<Result> ExecuteAsync<TCommand>(ICommandHandler<TCommand> handler, TCommand command, ICommandService commandService, CancellationToken token)
    {
        if (command is ContextBase context
            && !context.Settings.AllowGenerationWithoutProperties
            && !context.Settings.EnableInheritance
            && HasNoProperties(context.GetRequestModel()))
        {
            return Result.Invalid("There must be at least one property");
        }

        return (await _decoratee.ExecuteAsync(handler, command, commandService, token).ConfigureAwait(false))
            .OnSuccess(result =>
            {
                if (command is ContextBase context)
                {
                    var validationResults = new List<ValidationResult>();
                    if (!context.GetResponse().TryValidate(validationResults))
                    {
                        return Result.Invalid(validationResults.Select(x => new ValidationError(x.ErrorMessage, x.MemberNames)));
                    }
                }

                return result;
            });
    }

    public Task<Result<TResponse>> ExecuteAsync<TCommand, TResponse>(ICommandHandler<TCommand, TResponse> handler, TCommand command, ICommandService commandService, CancellationToken token)
        => _decoratee.ExecuteAsync(handler, command, commandService, token);

    private static bool HasNoProperties(object model)
    {
        if (model is TypeBase typeBase)
        {
            return typeBase.Properties.Count == 0;
        }

        if (model is Type t)
        {
            return t.GetProperties().Length == 0;
        }

        throw new NotSupportedException("Only TypeBase and Type are supported");
    }
}

namespace ClassFramework.Pipelines.CommandInterceptors;

public class ClassFrameworkCommandInterceptor : ICommandInterceptor
{
    public Task<Result> ExecuteAsync<TCommand>(TCommand command, ICommandService commandService, Func<Task<Result>> next, CancellationToken token)
    {
        next = ArgumentGuard.IsNotNull(next, nameof(next));

        return next();
    }

    public async Task<Result<TResponse>> ExecuteAsync<TCommand, TResponse>(TCommand command, ICommandService commandService, Func<Task<Result<TResponse>>> next, CancellationToken token)
    {
        next = ArgumentGuard.IsNotNull(next, nameof(next));

        if (command is CommandBase commandBase)
        {
            return await ExecuteClassFrameworkCommand(commandBase, next).ConfigureAwait(false);
        }

        return await next().ConfigureAwait(false);
    }

    private static async Task<Result<TResponse>> ExecuteClassFrameworkCommand<TResponse>(CommandBase commandBase, Func<Task<Result<TResponse>>> next)
    {
        if (!commandBase.Settings.AllowGenerationWithoutProperties
            && !commandBase.Settings.EnableInheritance
            && commandBase.SourceModelHasNoProperties())
        {
            return Result.Invalid<TResponse>("There must be at least one property");
        }

        return (await next().ConfigureAwait(false))
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

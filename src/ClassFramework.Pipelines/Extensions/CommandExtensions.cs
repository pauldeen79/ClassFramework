namespace ClassFramework.Pipelines.Extensions;

public static class CommandExtensions
{
    public static async Task<Result> ProcessPropertiesAsync<TCommand>(
        this TCommand command,
        string methodName,
        IEnumerable<Property> properties,
        Func<TCommand, Property, CancellationToken, Task<IReadOnlyDictionary<string, Result<GenericFormattableString>>>> getResultsDelegate,
        Func<string, string, string> getReturnTypeDelegate,
        Action<Property, string, IReadOnlyDictionary<string, Result<GenericFormattableString>>, string> processDelegate,
        CancellationToken token)
    {
        command = ArgumentGuard.IsNotNull(command, nameof(command));
        properties = ArgumentGuard.IsNotNull(properties, nameof(properties));
        getResultsDelegate = ArgumentGuard.IsNotNull(getResultsDelegate, nameof(getResultsDelegate));
        getReturnTypeDelegate = ArgumentGuard.IsNotNull(getReturnTypeDelegate, nameof(getReturnTypeDelegate));
        processDelegate = ArgumentGuard.IsNotNull(processDelegate, nameof(processDelegate));

        if (string.IsNullOrEmpty(methodName))
        {
            return Result.Success();
        }

        foreach (var property in properties)
        {
            var results = await getResultsDelegate(command, property, token).ConfigureAwait(false);

            var error = results.GetError();
            if (error is not null)
            {
                // Error in formattable string parsing
                return error;
            }

            var returnType = getReturnTypeDelegate(results.GetValue(ResultNames.Namespace), results.GetValue(ResultNames.BuilderName));

            processDelegate(property, returnType, results, returnType);
        }

        return Result.Success();
    }
}

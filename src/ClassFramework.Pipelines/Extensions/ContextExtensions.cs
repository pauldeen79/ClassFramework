namespace ClassFramework.Pipelines.Extensions;

public static class ContextExtensions
{
    public static async Task<Result> ProcessPropertiesAsync<TContext>(
        this TContext context,
        string methodName,
        IEnumerable<Property> properties,
        Func<TContext, Property, CancellationToken, Task<IReadOnlyDictionary<string, Result<GenericFormattableString>>>> getResultsDelegate,
        Func<string, string, string> getReturnTypeDelegate,
        Action<Property, string, IReadOnlyDictionary<string, Result<GenericFormattableString>>, string> processDelegate,
        CancellationToken token)
    {
        context = ArgumentGuard.IsNotNull(context, nameof(context));
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
            var results = await getResultsDelegate(context, property, token).ConfigureAwait(false);

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

internal sealed class ConstructionMethodParameterInfo
{
    public string Name { get; }
    public Property Source { get; }
    public Result<GenericFormattableString> Result { get; }
    public string CollectionInitializer { get; }
    public string Suffix { get; }
    public bool UseBuilderLazyValues { get; }

    public ConstructionMethodParameterInfo(string name, Property source, Result<GenericFormattableString> result, string collectionInitializer, string suffix, bool useBuilderLazyValues)
    {
        Name = name;
        Source = source;
        Result = result;
        CollectionInitializer = collectionInitializer;
        Suffix = suffix;
        UseBuilderLazyValues = useBuilderLazyValues;
    }
}

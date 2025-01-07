namespace ClassFramework.Pipelines.Extensions;

public static class ResultDictionaryExtensions
{
    public static Result<T> GetError<T>(this Dictionary<string, Result<T>> resultDictionary)
        => resultDictionary.Select(x => x.Value).FirstOrDefault(x => !x.IsSuccessful());

    public static Result OnSuccess<T>(this Dictionary<string, Result<T>> resultDictionary, Func<Dictionary<string, Result<T>>, Result> successDelegate)
    {
        successDelegate = ArgumentGuard.IsNotNull(successDelegate, nameof(successDelegate));

        var error = resultDictionary.GetError();

        return error switch
        {
            not null => error,
            _ => successDelegate(resultDictionary)
        };
    }

    public static Result OnSuccess<T>(this Dictionary<string, Result<T>> resultDictionary, Action<Dictionary<string, Result<T>>> successDelegate)
        => resultDictionary.OnSuccess(_ => { successDelegate(resultDictionary); return Result.Success(); });
}

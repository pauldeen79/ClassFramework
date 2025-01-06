namespace ClassFramework.Pipelines.Extensions;

public static class ResultDictionaryExtensions
{
    public static Result<T> GetError<T>(this Dictionary<string, Result<T>> resultDictionary)
        => resultDictionary.Select(x => x.Value).FirstOrDefault(x => !x.IsSuccessful());
}

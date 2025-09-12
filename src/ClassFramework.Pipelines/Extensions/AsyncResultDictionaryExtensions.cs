namespace ClassFramework.Pipelines.Extensions;

public static class AsyncResultDictionaryExtensions
{
    public static bool NeedNonLazyOverloads(this IReadOnlyDictionary<string, Result<GenericFormattableString>> instance)
        => instance.GetValue(ResultNames.TypeName) != instance.GetValue(ResultNames.NonLazyTypeName);
}

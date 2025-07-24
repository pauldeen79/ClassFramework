namespace ClassFramework.Pipelines.Extensions;

public static class AsyncResultDictionaryExtensions
{
    public static bool NeedNonLazyOverloads(this IReadOnlyDictionary<string, Result<GenericFormattableString>> instance)
        //TODO: Add functionality to GenericFormattableString, so we can compare them by value (just like string)
        => instance.GetValue(ResultNames.TypeName).ToString() != instance.GetValue(ResultNames.NonLazyTypeName).ToString();
}

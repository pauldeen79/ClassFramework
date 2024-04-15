namespace ClassFramework.Pipelines.Extensions;

public static class EnumerableOfMetadataExtensions
{
    public static string GetStringValue(this IEnumerable<Metadata> metadata, string metadataName, string defaultValue = "")
        => metadata.GetStringValue(metadataName, () => defaultValue);

    public static string GetStringValue(this IEnumerable<Metadata> metadata, string metadataName, Func<string> defaultValueDelegate)
        => metadata.GetValue<object?>(metadataName, defaultValueDelegate).ToStringWithDefault(defaultValueDelegate());

    public static Result<string> GetStringResult(this IEnumerable<Metadata> metadata, string metadataName, Func<Result<string>> defaultValueDelegate)
    {
        defaultValueDelegate = defaultValueDelegate.IsNotNull(nameof(defaultValueDelegate));

        var value = metadata.GetValue<object?>(metadataName, () => null).ToStringWithDefault();
        
        return string.IsNullOrEmpty(value)
            ? defaultValueDelegate()
            : Result.Success(value);
    }

    public static Result<FormattableStringParserResult> GetFormattableStringParserResult(this IEnumerable<Metadata> metadata, string metadataName, Func<Result<FormattableStringParserResult>> defaultValueDelegate)
    {
        defaultValueDelegate = defaultValueDelegate.IsNotNull(nameof(defaultValueDelegate));

        var value = metadata.GetValue<object?>(metadataName, () => null).ToStringWithDefault();

        return string.IsNullOrEmpty(value)
            ? defaultValueDelegate()
            : Result.Success<FormattableStringParserResult>(value);
    }

    public static bool GetBooleanValue(this IEnumerable<Metadata> metadata, string metadataName, bool defaultValue = false)
        => metadata.GetBooleanValue(metadataName, () => defaultValue);

    public static bool GetBooleanValue(this IEnumerable<Metadata> metadata, string metadataName, Func<bool> defaultValueDelegate)
        => metadata.GetValue<object?>(metadataName, () => defaultValueDelegate.Invoke()).ToStringWithDefault().IsTrue();

    public static T GetValue<T>(this IEnumerable<Metadata> metadata, string metadataName, Func<T> defaultValueDelegate)
    {
        defaultValueDelegate = defaultValueDelegate.IsNotNull(nameof(defaultValueDelegate));

        var metadataItem = metadata.LastOrDefault(md => md.Name == metadataName);

        if (metadataItem is null)
        {
            return defaultValueDelegate();
        }

        return GetValue(metadataItem, defaultValueDelegate);
    }

    public static IEnumerable<string> GetStringValues(this IEnumerable<Metadata> metadata, string metadataName)
        => metadata.GetValues<object?>(metadataName).Select(x => x.ToStringWithDefault());

    public static IEnumerable<T> GetValues<T>(this IEnumerable<Metadata> metadata, string metadataName)
        => metadata
            .Where(md => md.Name == metadataName)
            .Select(md => md.Value)
            .OfType<T>();

    private static T GetValue<T>(Metadata metadataItem, Func<T> defaultValueDelegate)
    {
        if (metadataItem.Value is T t)
        {
            return t;
        }

        if (typeof(T).IsEnum)
        {
            var val = metadataItem.Value.ToStringWithNullCheck();
            return string.IsNullOrEmpty(val)
                ? defaultValueDelegate()
                : (T)Enum.Parse(typeof(T), val, true);
        }

        if (typeof(T).FullName.StartsWith("System.Nullable`1[[") && typeof(T).GetGenericArguments()[0].IsEnum)
        {
            var val = metadataItem.Value.ToStringWithNullCheck();
            return string.IsNullOrEmpty(val)
                ? defaultValueDelegate()
                : (T)Enum.Parse(typeof(T).GetGenericArguments()[0], val, true);
        }

        return (T)Convert.ChangeType(metadataItem.Value, typeof(T));
    }
}

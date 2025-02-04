namespace ClassFramework.Domain.Extensions;

public static class EnumerableOfStringExtensions
{
    public static string GetGenericTypeArgumentsString(this IEnumerable<string> instance, bool addBrackets = true)
    {
        var prefix = addBrackets ? "<" : string.Empty;
        var suffix = addBrackets ? ">" : string.Empty;

        var items = instance.ToArray();
        return items.Length > 0
            ? $"{prefix}{string.Join(", ", items)}{suffix}"
            : string.Empty;
    }
}

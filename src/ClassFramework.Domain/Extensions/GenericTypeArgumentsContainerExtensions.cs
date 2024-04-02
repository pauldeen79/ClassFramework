namespace ClassFramework.Domain.Extensions;

public static class GenericTypeArgumentsContainerExtensions
{
    public static string GetGenericTypeArgumentsString(this IGenericTypeArgumentsContainer instance, bool addBrackets = true)
    {
        var prefix = addBrackets ? "<" : string.Empty;
        var suffix = addBrackets ? ">" : string.Empty;

        return instance.GenericTypeArguments.Count > 0
            ? $"{prefix}{string.Join(", ", instance.GenericTypeArguments)}{suffix}"
            : string.Empty;
    }

    public static string GetGenericTypeArgumentConstraintsString(this IGenericTypeArgumentsContainer instance, int indent)
        => instance.GenericTypeArgumentConstraints.Count > 0
            ? string.Concat(Environment.NewLine,
                            new string(' ', indent),
                            string.Join(string.Concat(Environment.NewLine, new string(' ', indent)), instance.GenericTypeArgumentConstraints))
            : string.Empty;
}

namespace ClassFramework.Domain.Extensions;

public static class GenericTypeArgumentsContainerExtensions
{
    public static string GetGenericTypeArgumentsString(this IGenericTypeArgumentsContainer instance, bool addBrackets = true)
        => instance.GenericTypeArguments.GetGenericTypeArgumentsString(addBrackets);

    public static string GetGenericTypeArgumentConstraintsString(this IGenericTypeArgumentsContainer instance, int indent)
        => instance.GenericTypeArgumentConstraints.Count > 0
            ? string.Concat(Environment.NewLine,
                            new string(' ', indent),
                            string.Join(string.Concat(Environment.NewLine, new string(' ', indent)), instance.GenericTypeArgumentConstraints))
            : string.Empty;
}

namespace ClassFramework.Domain;

public static class MethodExtensions
{
    public static bool IsInterfaceMethod(this Method instance)
        => instance.Name.StartsWith("I", StringComparison.Ordinal)
        && instance.Name.Length > 1
        && instance.Name.Contains('.');
}

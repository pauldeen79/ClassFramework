namespace ClassFramework.TemplateFramework.Extensions;

public static class TypeExtensions
{
    public static string GetEntityClassName(this Type instance)
        => (instance.IsInterface
            && instance.Name.StartsWith('I')
            && instance.Name.Length >= 2
            && instance.Name.Substring(1, 1).Equals(instance.Name.Substring(1, 1).ToUpperInvariant(), StringComparison.Ordinal)
            ? instance.Name[1..]
            : instance.Name).WithoutTypeGenerics();
}

namespace ClassFramework.Pipelines.Extensions;

public static class TypeContainerExtensions
{
    public static bool HasNonDefaultDefaultValue(this ITypeContainer container)
        => !container.TypeName.GetDefaultValue(container.IsNullable, container.IsValueType, false).StartsWith("default(", StringComparison.Ordinal);

    public static bool IsNullable(this ITypeContainer container, bool enableNullableReferenceTypes)
        => container.IsValueType
            ? container.IsNullable || container.TypeName.StartsWith("System.Nullable")
            : enableNullableReferenceTypes && container.IsNullable;
}

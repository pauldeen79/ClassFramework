namespace ClassFramework.Pipelines.Extensions;

public static class TypeNameMapperExtensions
{
    public static string MapTypeName(this ITypeNameMapper instance, string typeName)
        => instance.MapTypeName(typeName, string.Empty);
}

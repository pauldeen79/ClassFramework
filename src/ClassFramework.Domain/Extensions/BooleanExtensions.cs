namespace ClassFramework.Domain.Extensions;

public static  class BooleanExtensions
{
    public static string GetLazyPrefix(this bool useBuilderLazyValues, string typeName)
    => useBuilderLazyValues
        ? $"new {typeName.ToLazy()}(() => "
        : string.Empty;

    public static string GetLazySuffix(this bool useBuilderLazyValues)
        => useBuilderLazyValues
            ? ")"
            : string.Empty;
}

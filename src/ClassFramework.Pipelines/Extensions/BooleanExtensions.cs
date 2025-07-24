namespace ClassFramework.Pipelines.Extensions;

public static class BooleanExtensions
{
    public static Visibility ToVisibility(this bool isPublic)
        => isPublic
            ? Visibility.Public
            : Visibility.Private;

    public static string GetLazyPrefix(this bool useBuilderLazyValues, string typeName)
        => useBuilderLazyValues
            ? $"new {typeName.WrapDelegate()}(() => "
            : string.Empty;

    public static string GetLazySuffix(this bool useBuilderLazyValues)
        => useBuilderLazyValues
            ? ")"
            : string.Empty;
}

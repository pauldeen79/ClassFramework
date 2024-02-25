namespace ClassFramework.Pipelines.Extensions;

public static class BooleanExtensions
{
    public static Visibility ToVisibility(this bool isPublic)
        => isPublic
            ? Visibility.Public
            : Visibility.Private;
}

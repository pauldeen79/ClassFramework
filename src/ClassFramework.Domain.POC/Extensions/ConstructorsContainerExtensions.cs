namespace ClassFramework.Domain.Extensions;

public static class ConstructorsContainerExtensions
{
    public static bool HasPublicParameterlessConstructor(this IConstructorsContainer instance)
        => instance.Constructors.Count == 0 || instance.Constructors.Any(x => x.Parameters.Count == 0 && x.Visibility == Visibility.Public);
}

namespace ClassFramework.Domain.Builders.Extensions;

public static partial class BaseClassContainerBuilderExtensions
{
    public static T WithBaseClass<T>(this T instance, Type baseClassType)
        where T : IBaseClassContainerBuilder
        => instance.WithBaseClass(baseClassType.IsNotNull(nameof(baseClassType)).FullName);
}

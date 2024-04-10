namespace ClassFramework.Pipelines.Extensions;

public static class TypeContainerBuilderExtensions
{
    public static T SetTypeContainerPropertiesFrom<T>(this T instance, ITypeContainer member)
        where T : ITypeContainerBuilder
    {
        member = member.IsNotNull(nameof(member));

        return instance
            .WithIsNullable(member.IsNullable)
            .WithIsValueType(member.IsValueType)
            .AddGenericTypeArguments(member.GenericTypeArguments);
    }
}

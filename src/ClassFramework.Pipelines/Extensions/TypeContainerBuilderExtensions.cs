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

    public static T SetTypeContainerPropertiesFrom<T>(this T instance, bool isNullable, Type memberType, Func<Type, MemberInfo, string> mapDelegate)
        where T : ITypeContainerBuilder
    {
        memberType = memberType.IsNotNull(nameof(memberType));
        mapDelegate = mapDelegate.IsNotNull(nameof(mapDelegate));

        return instance
            .WithIsNullable(isNullable)
            .WithIsValueType(memberType.IsValueType())
            .AddGenericTypeArguments(memberType.GenericTypeArguments.Select((x, index) => x.ToTypeContainer(memberType, index + 1, mapDelegate)));
    }
}

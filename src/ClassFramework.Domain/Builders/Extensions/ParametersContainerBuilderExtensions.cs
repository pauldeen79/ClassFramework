namespace ClassFramework.Domain.Builders.Extensions;

public static partial class ParametersContainerBuilderExtensions
{
    public static T AddParameter<T>(this T instance, string name, Type type) where T : IParametersContainerBuilder
    {
        instance.Parameters.Add(new ParameterBuilder().WithName(name.IsNotNull(nameof(name))).WithType(type.IsNotNull(nameof(type))));
        return instance;
    }

    public static T AddParameter<T>(this T instance, string name, Type type, bool isNullable) where T : IParametersContainerBuilder
    {
        instance.Parameters.Add(new ParameterBuilder().WithName(name.IsNotNull(nameof(name))).WithType(type.IsNotNull(nameof(type))).WithIsNullable(isNullable));
        return instance;
    }

    public static T AddParameter<T>(this T instance, string name, string typeName) where T : IParametersContainerBuilder
    {
        instance.Parameters.Add(new ParameterBuilder().WithName(name.IsNotNull(nameof(name))).WithTypeName(typeName));
        return instance;
    }

    public static T AddParameter<T>(this T instance, string name, string typeName, bool isNullable) where T : IParametersContainerBuilder
    {
        instance.Parameters.Add(new ParameterBuilder().WithName(name.IsNotNull(nameof(name))).WithTypeName(typeName).WithIsNullable(isNullable));
        return instance;
    }
}

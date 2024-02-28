namespace ClassFramework.Domain.Builders;

public partial class MethodBuilder
{
    public MethodBuilder WithReturnType(Type type)
    {
        type = type.IsNotNull(nameof(type));

        ReturnTypeName = type.FullName.FixTypeName();
        ReturnTypeIsValueType = type.IsValueType;

        return this;
    }

    public MethodBuilder WithReturnType(ITypeBuilder typeBuilder)
    {
        typeBuilder = typeBuilder.IsNotNull(nameof(typeBuilder));

        ReturnTypeName = typeBuilder.GetFullName();
        ReturnTypeIsValueType = typeBuilder is IValueTypeBuilder;

        return this;
    }
}

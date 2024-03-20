namespace ClassFramework.Domain.Builders;

[CustomValidation(typeof(PropertyValidator), nameof(PropertyValidator.Validate))]
public partial class PropertyBuilder
{
    // note that this code should not be necesary, unless you are not storing generated code in the GIT repository, and you are still using the Bootstrap code generation project ;-)
    partial void SetDefaultValues()
    {
        HasGetter = true;
        HasSetter = true;
    }

    public PropertyBuilder WithParentType(Type parentType) => WithParentTypeFullName(parentType.IsNotNull(nameof(parentType)).FullName.FixTypeName());
    public PropertyBuilder WithParentType(IType parentType) => WithParentTypeFullName(parentType.IsNotNull(nameof(parentType)).GetFullName());
    public PropertyBuilder WithParentType(ITypeBuilder parentType) => WithParentTypeFullName(parentType.IsNotNull(nameof(parentType)).GetFullName());
}

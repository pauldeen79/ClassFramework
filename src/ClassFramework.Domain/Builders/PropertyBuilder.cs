namespace ClassFramework.Domain.Builders;

[CustomValidation(typeof(PropertyValidator), nameof(PropertyValidator.Validate))]
public partial class PropertyBuilder
{
    partial void SetDefaultValues()
    {
        HasGetter = true;
        HasSetter = true;
    }
}

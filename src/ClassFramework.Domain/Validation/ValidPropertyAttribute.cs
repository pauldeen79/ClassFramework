namespace ClassFramework.Domain.Validation;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class ValidPropertyAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if ((value is Property || value is PropertyBuilder)
            && value.GetType().GetProperty(nameof(Property.HasSetter)).GetValue(value) is bool b1 && b1
            && value.GetType().GetProperty(nameof(Property.HasInitializer)).GetValue(value) is bool b2 && b2)
        {
            return new ValidationResult($"{nameof(Property.HasSetter)} and {nameof(Property.HasInitializer)} cannot both be true", [nameof(Property.HasSetter), nameof(Property.HasInitializer)]);
        }

        return ValidationResult.Success;
    }
}

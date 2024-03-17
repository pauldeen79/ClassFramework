namespace ClassFramework.Domain;

[CustomValidation(typeof(PropertyValidator), nameof(PropertyValidator.Validate))]
public partial record Property
{
}

public static class PropertyValidator
{
    public static ValidationResult Validate(object instance)
    {
        if (instance is null)
        {
            return ValidationResult.Success;
        }

        if (instance.GetType().GetProperty(nameof(Property.HasSetter)).GetValue(instance) is bool b1 && b1
            && instance.GetType().GetProperty(nameof(Property.HasInitializer)).GetValue(instance) is bool b2 && b2)
        {
            return new ValidationResult($"{nameof(Property.HasSetter)} and {nameof(Property.HasInitializer)} cannot both be true", [nameof(Property.HasSetter), nameof(Property.HasInitializer)]);
        }

        return ValidationResult.Success;
    }
}

namespace ClassFramework.Domain.Validation;

public static class PropertyValidator
{
    public static ValidationResult Validate(object instance)
    {
        if (instance is null)
        {
            return ValidationResult.Success;
        }

        if (!instance.GetType().Name.In(nameof(Property), nameof(PropertyBuilder)))
        {
            return new ValidationResult($"The {nameof(PropertyValidator)} attribute can only be applied to {nameof(Property)} and {nameof(PropertyBuilder)} types");
        }

        if (instance.GetType().GetProperty(nameof(Property.HasSetter)).GetValue(instance) is bool b1 && b1
            && instance.GetType().GetProperty(nameof(Property.HasInitializer)).GetValue(instance) is bool b2 && b2)
        {
            return new ValidationResult($"{nameof(Property.HasSetter)} and {nameof(Property.HasInitializer)} cannot both be true", [nameof(Property.HasSetter), nameof(Property.HasInitializer)]);
        }

        return ValidationResult.Success;
    }
}

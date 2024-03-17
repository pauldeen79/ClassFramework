namespace ClassFramework.Domain;

[CustomValidation(typeof(PropertyValidator), nameof(PropertyValidator.Validate))]
public partial record Property
{
}

public static class PropertyValidator
{
    public static ValidationResult Validate(object instance, ValidationContext context)
    {
        if (context?.ObjectInstance is null)
        {
            return ValidationResult.Success;
        }

        if (context.ObjectType.GetProperty(nameof(Property.HasSetter)).GetValue(context.ObjectInstance) is bool b1 && b1
            && context.ObjectType.GetProperty(nameof(Property.HasInitializer)).GetValue(context.ObjectInstance) is bool b2 && b2)
        {
            return new ValidationResult("HasSetter and HasInitializer cannot both be true", [nameof(Property.HasSetter), nameof(Property.HasInitializer)]);
        }

        return ValidationResult.Success;
    }
}

namespace ClassFramework.Domain;

[CustomValidation(typeof(PropertyValidator), nameof(PropertyValidator.Validate))]
public partial record Property
{
}

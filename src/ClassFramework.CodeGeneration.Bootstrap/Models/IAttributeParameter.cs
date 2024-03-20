namespace ClassFramework.CodeGeneration.Bootstrap.Models;

internal interface IAttributeParameter
{
    [Required(AllowEmptyStrings = true)] string Name { get; }
    object Value { get; }
}

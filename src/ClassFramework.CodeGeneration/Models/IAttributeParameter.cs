namespace ClassFramework.CodeGeneration.Models;

internal interface IAttributeParameter : Abstractions.IMetadataContainer
{
    [Required(AllowEmptyStrings = true)] string Name { get; }
    object Value { get; }
}

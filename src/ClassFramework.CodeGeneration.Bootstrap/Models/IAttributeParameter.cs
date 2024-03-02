namespace ClassFramework.CodeGeneration.Models;

internal interface IAttributeParameter : IMetadataContainer
{
    [Required(AllowEmptyStrings = true)] string Name { get; }
    object Value { get; }
}

namespace ClassFramework.CodeGeneration.Bootstrap.Models;

internal interface IAttributeParameter : IMetadataContainer
{
    [Required(AllowEmptyStrings = true)] string Name { get; }
    object Value { get; }
}

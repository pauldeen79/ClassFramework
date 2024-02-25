namespace ClassFramework.IntegrationTests.Models;

internal interface IAttributeParameter : Abstractions.IMetadataContainer
{
    [Required(AllowEmptyStrings = true)] string Name { get; }
    object Value { get; }
}

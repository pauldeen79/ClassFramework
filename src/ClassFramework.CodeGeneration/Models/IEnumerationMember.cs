namespace ClassFramework.CodeGeneration.Models;

internal interface IEnumerationMember : IAttributesContainer, INameContainer, IMetadataContainer
{
    object? Value { get; }
}

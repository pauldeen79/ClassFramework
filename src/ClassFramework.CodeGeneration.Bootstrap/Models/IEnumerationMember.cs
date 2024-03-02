namespace ClassFramework.CodeGeneration.Bootstrap.Models;

internal interface IEnumerationMember : IAttributesContainer, INameContainer, IMetadataContainer
{
    object? Value { get; }
}

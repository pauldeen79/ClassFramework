namespace ClassFramework.IntegrationTests.Models;

internal interface IEnumerationMember : Abstractions.IAttributesContainer, Abstractions.INameContainer, Abstractions.IMetadataContainer
{
    object? Value { get; }
}

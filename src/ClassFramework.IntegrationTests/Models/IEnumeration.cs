namespace ClassFramework.IntegrationTests.Models;

internal interface IEnumeration : Abstractions.IAttributesContainer, Abstractions.IMetadataContainer, Abstractions.INameContainer, Abstractions.IVisibilityContainer
{
    [Required] IReadOnlyCollection<IEnumerationMember> Members { get; }
}

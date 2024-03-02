namespace ClassFramework.CodeGeneration.Bootstrap.Models;

internal interface IEnumeration : IAttributesContainer, IMetadataContainer, INameContainer, IVisibilityContainer
{
    [Required] IReadOnlyCollection<IEnumerationMember> Members { get; }
}

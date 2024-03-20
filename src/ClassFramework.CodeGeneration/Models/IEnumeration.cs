namespace ClassFramework.CodeGeneration.Models;

internal interface IEnumeration : Abstractions.IAttributesContainer, Abstractions.INameContainer, Abstractions.IVisibilityContainer
{
    [Required] IReadOnlyCollection<IEnumerationMember> Members { get; }
}

namespace ClassFramework.CodeGeneration.Models;

internal interface IEnumeration : Abstractions.IAttributesContainer, Abstractions.INameContainer, Abstractions.IVisibilityContainer
{
    [Required] [ValidateObject] IReadOnlyCollection<IEnumerationMember> Members { get; }
}

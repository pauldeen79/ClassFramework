namespace ClassFramework.CodeGeneration.Bootstrap.Models;

internal interface IEnumeration : IAttributesContainer, INameContainer, IVisibilityContainer
{
    [Required] IReadOnlyCollection<IEnumerationMember> Members { get; }
}

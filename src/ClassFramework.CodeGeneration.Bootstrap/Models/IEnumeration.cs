namespace ClassFramework.CodeGeneration.Bootstrap.Models;

internal interface IEnumeration : IAttributesContainer, INameContainer, IVisibilityContainer
{
    [Required] [ValidateObject] IReadOnlyCollection<IEnumerationMember> Members { get; }
}

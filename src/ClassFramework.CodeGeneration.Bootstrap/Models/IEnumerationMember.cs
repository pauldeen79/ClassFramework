namespace ClassFramework.CodeGeneration.Bootstrap.Models;

internal interface IEnumerationMember : IAttributesContainer, INameContainer
{
    object? Value { get; }
}

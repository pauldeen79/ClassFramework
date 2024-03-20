namespace ClassFramework.CodeGeneration.Models;

internal interface IEnumerationMember : Abstractions.IAttributesContainer, Abstractions.INameContainer
{
    object? Value { get; }
}

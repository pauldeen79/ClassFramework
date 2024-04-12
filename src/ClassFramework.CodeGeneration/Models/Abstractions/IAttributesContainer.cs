namespace ClassFramework.CodeGeneration.Models.Abstractions;

internal interface IAttributesContainer
{
    [Required][ValidateObject] IReadOnlyCollection<IAttribute> Attributes { get; }
}

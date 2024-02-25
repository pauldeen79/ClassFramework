namespace ClassFramework.CodeGeneration.Models.Abstractions;

internal interface IAttributesContainer
{
    [Required] IReadOnlyCollection<IAttribute> Attributes { get; }
}

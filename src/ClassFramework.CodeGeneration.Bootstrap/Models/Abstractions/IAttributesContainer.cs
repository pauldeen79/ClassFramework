namespace ClassFramework.CodeGeneration.Bootstrap.Models.Abstractions;

internal interface IAttributesContainer
{
    [Required] IReadOnlyCollection<IAttribute> Attributes { get; }
}

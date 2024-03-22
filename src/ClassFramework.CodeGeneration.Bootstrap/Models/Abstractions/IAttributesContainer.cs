namespace ClassFramework.CodeGeneration.Bootstrap.Models.Abstractions;

internal interface IAttributesContainer
{
    [Required] [ValidateObject] IReadOnlyCollection<IAttribute> Attributes { get; }
}

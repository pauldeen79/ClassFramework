namespace ClassFramework.IntegrationTests.Models.Abstractions;

internal interface IAttributesContainer
{
    [Required] IReadOnlyCollection<IAttribute> Attributes { get; }
}

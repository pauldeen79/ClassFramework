namespace ClassFramework.IntegrationTests.Models;

internal interface IAttribute : Abstractions.IMetadataContainer, Abstractions.INameContainer
{
    [Required] IReadOnlyCollection<IAttributeParameter> Parameters { get; }
}

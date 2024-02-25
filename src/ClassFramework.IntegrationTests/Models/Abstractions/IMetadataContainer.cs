namespace ClassFramework.IntegrationTests.Models.Abstractions;

internal interface IMetadataContainer
{
    [Required] IReadOnlyCollection<IMetadata> Metadata { get; }
}

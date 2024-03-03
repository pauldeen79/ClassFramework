namespace ClassFramework.CodeGeneration.Bootstrap.Models.Abstractions;

internal interface IMetadataContainer
{
    [Required] IReadOnlyCollection<IMetadata> Metadata { get; }
}

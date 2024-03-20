namespace ClassFramework.CodeGeneration.Models.Pipelines;

internal interface IMetadataContainer
{
    [Required] IReadOnlyCollection<IMetadata> Metadata { get; }
}

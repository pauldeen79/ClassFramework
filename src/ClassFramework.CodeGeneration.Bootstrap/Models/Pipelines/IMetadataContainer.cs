namespace ClassFramework.CodeGeneration.Bootstrap.Models.Pipelines;

internal interface IMetadataContainer
{
    [Required] IReadOnlyCollection<IMetadata> Metadata { get; }
}

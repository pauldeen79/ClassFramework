namespace ClassFramework.CodeGeneration.Models.Abstractions;

internal interface IMetadataContainer
{
    [Required] IReadOnlyCollection<IMetadata> Metadata { get; }
}

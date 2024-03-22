namespace ClassFramework.CodeGeneration.Models.Pipelines;

internal interface INamespaceMapping
{
    [Required] string SourceNamespace { get; }
    [Required] string TargetNamespace { get; }
    [Required] [ValidateObject] IReadOnlyCollection<IMetadata> Metadata { get; }
}

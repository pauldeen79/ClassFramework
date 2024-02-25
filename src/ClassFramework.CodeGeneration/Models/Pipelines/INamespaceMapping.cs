namespace ClassFramework.CodeGeneration.Models.Pipelines;

internal interface INamespaceMapping : IMetadataContainer
{
    [Required] string SourceNamespace { get; }
    [Required] string TargetNamespace { get; }
}

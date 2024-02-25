namespace ClassFramework.IntegrationTests.Models.Pipelines;

internal interface INamespaceMapping : Abstractions.IMetadataContainer
{
    [Required] string SourceNamespace { get; }
    [Required] string TargetNamespace { get; }
}

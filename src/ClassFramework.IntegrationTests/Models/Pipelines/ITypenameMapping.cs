namespace ClassFramework.IntegrationTests.Models.Pipelines;

internal interface ITypenameMapping : Abstractions.IMetadataContainer
{
    [Required] string SourceTypeName { get; }
    [Required] string TargetTypeName { get; }
}

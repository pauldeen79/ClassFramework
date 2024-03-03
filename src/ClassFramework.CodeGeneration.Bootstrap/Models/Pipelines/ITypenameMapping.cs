namespace ClassFramework.CodeGeneration.Bootstrap.Models.Pipelines;

internal interface ITypenameMapping : IMetadataContainer
{
    [Required] string SourceTypeName { get; }
    [Required] string TargetTypeName { get; }
}

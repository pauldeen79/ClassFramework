namespace ClassFramework.CodeGeneration.Models.Pipelines;

//TODO: Restore inheritance after making abstractions namespace configurable
internal interface IMetadata //: Abstractions.INameContainer
{
    object? Value { get; }
    [Required] string Name { get; }
}

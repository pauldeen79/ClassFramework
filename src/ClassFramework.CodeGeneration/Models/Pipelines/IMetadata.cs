namespace ClassFramework.CodeGeneration.Models.Pipelines;

internal interface IMetadata : Abstractions.INameContainer
{
    object? Value { get; }
}

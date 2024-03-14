namespace ClassFramework.CodeGeneration.Models.Abstractions;

internal interface INameContainer
{
    [Required] INameIdentifier Name { get; }
}

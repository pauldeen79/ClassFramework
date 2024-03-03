namespace ClassFramework.CodeGeneration.Models;

internal interface IAttribute : Abstractions.IMetadataContainer, Abstractions.INameContainer
{
    [Required] IReadOnlyCollection<IAttributeParameter> Parameters { get; }
}

namespace ClassFramework.CodeGeneration.Models;

internal interface IAttribute : IMetadataContainer, INameContainer
{
    [Required] IReadOnlyCollection<IAttributeParameter> Parameters { get; }
}

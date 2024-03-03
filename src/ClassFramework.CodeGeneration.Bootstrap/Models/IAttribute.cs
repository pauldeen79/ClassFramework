namespace ClassFramework.CodeGeneration.Bootstrap.Models;

internal interface IAttribute : IMetadataContainer, INameContainer
{
    [Required] IReadOnlyCollection<IAttributeParameter> Parameters { get; }
}

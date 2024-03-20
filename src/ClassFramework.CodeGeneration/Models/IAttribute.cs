namespace ClassFramework.CodeGeneration.Models;

internal interface IAttribute : Abstractions.INameContainer
{
    [Required] IReadOnlyCollection<IAttributeParameter> Parameters { get; }
}

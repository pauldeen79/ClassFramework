namespace ClassFramework.CodeGeneration.Models;

internal interface IAttribute : Abstractions.INameContainer
{
    [Required][ValidateObject] IReadOnlyCollection<IAttributeParameter> Parameters { get; }
}

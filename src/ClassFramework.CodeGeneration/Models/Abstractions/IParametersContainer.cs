namespace ClassFramework.CodeGeneration.Models.Abstractions;

internal interface IParametersContainer
{
    [Required] [ValidateObject] IReadOnlyCollection<IParameter> Parameters { get; }
}

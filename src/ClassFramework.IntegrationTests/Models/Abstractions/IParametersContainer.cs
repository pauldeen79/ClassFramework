namespace ClassFramework.CodeGeneration.Models.Abstractions;

internal interface IParametersContainer
{
    [Required] IReadOnlyCollection<IParameter> Parameters { get; }
}

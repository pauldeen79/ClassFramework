namespace ClassFramework.CodeGeneration.Bootstrap.Models.Abstractions;

internal interface IParametersContainer
{
    [Required] IReadOnlyCollection<IParameter> Parameters { get; }
}

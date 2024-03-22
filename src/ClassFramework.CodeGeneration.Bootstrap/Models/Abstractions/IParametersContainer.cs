namespace ClassFramework.CodeGeneration.Bootstrap.Models.Abstractions;

internal interface IParametersContainer
{
    [Required] [ValidateObject] IReadOnlyCollection<IParameter> Parameters { get; }
}

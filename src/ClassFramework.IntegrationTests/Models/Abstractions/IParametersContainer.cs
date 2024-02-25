namespace ClassFramework.IntegrationTests.Models.Abstractions;

internal interface IParametersContainer
{
    [Required] IReadOnlyCollection<IParameter> Parameters { get; }
}

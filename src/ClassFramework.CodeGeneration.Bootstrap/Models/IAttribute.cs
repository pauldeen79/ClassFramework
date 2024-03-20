namespace ClassFramework.CodeGeneration.Bootstrap.Models;

internal interface IAttribute : INameContainer
{
    [Required] IReadOnlyCollection<IAttributeParameter> Parameters { get; }
}

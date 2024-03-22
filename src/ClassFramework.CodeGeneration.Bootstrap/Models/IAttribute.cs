namespace ClassFramework.CodeGeneration.Bootstrap.Models;

internal interface IAttribute : INameContainer
{
    [Required] [ValidateObject] IReadOnlyCollection<IAttributeParameter> Parameters { get; }
}

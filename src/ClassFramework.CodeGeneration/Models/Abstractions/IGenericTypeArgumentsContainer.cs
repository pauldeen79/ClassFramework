namespace ClassFramework.CodeGeneration.Models.Abstractions;

internal interface IGenericTypeArgumentsContainer
{
    [Required] [ValidateObject] IReadOnlyCollection<string> GenericTypeArguments { get; }
    [Required] [ValidateObject] IReadOnlyCollection<string> GenericTypeArgumentConstraints { get; }
}

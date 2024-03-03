namespace ClassFramework.CodeGeneration.Bootstrap.Models.Abstractions;

internal interface IGenericTypeArgumentsContainer
{
    [Required] IReadOnlyCollection<string> GenericTypeArguments { get; }
    [Required] IReadOnlyCollection<string> GenericTypeArgumentConstraints { get; }
}

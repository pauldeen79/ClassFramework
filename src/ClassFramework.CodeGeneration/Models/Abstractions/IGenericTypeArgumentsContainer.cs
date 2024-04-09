namespace ClassFramework.CodeGeneration.Models.Abstractions;

internal interface IGenericTypeArgumentsContainer
{
    [Required][ValidateObject] IReadOnlyCollection<ITypeInfo> GenericTypeArguments { get; }
    [Required][ValidateObject] IReadOnlyCollection<string> GenericTypeArgumentConstraints { get; }
}

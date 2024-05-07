namespace ClassFramework.CodeGeneration.Models;

internal interface IMethod : Abstractions.IModifiersContainer, Abstractions.INameContainer, Abstractions.IAttributesContainer, Abstractions.ICodeStatementsContainer, Abstractions.IParametersContainer, Abstractions.IExplicitInterfaceNameContainer, Abstractions.IParentTypeContainer, Abstractions.IGenericTypeArgumentsContainer, Abstractions.ISuppressWarningCodesContainer
{
    [Required(AllowEmptyStrings = true)] string ReturnTypeName { get; }
    bool ReturnTypeIsNullable { get; }
    bool ReturnTypeIsValueType { get; }
    [Required][ValidateObject] IReadOnlyCollection<ITypeContainer> ReturnTypeGenericTypeArguments { get; }

    bool Partial { get; }
    bool ExtensionMethod { get; }
    bool Operator { get; }
    bool Async { get; }
}

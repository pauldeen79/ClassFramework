namespace ClassFramework.CodeGeneration.Models;

internal interface IMethod : IMetadataContainer, IExtendedVisibilityContainer, INameContainer, IAttributesContainer, ICodeStatementsContainer, IParametersContainer, IExplicitInterfaceNameContainer, IParentTypeContainer, IGenericTypeArgumentsContainer
{
    [Required(AllowEmptyStrings = true)] string ReturnTypeName { get; }
    bool ReturnTypeIsNullable { get; }
    bool ReturnTypeIsValueType { get; }

    bool Partial { get; }
    bool ExtensionMethod { get; }
    bool Operator { get; }
    bool Async { get; }
}

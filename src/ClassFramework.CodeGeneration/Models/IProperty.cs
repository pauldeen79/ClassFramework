namespace ClassFramework.CodeGeneration.Models;

[ValidProperty]
internal interface IProperty : Abstractions.IModifiersContainer, Abstractions.INameContainer, Abstractions.IAttributesContainer, Abstractions.ITypeContainer, Abstractions.IDefaultValueContainer, Abstractions.IExplicitInterfaceNameContainer, Abstractions.IParentTypeContainer
{
    [DefaultValue(true)] bool HasGetter { get; }
    [DefaultValue(true)] bool HasSetter { get; }
    bool HasInitializer { get; }
    SubVisibility GetterVisibility { get; }
    SubVisibility SetterVisibility { get; }
    SubVisibility InitializerVisibility { get; }
    [Required][ValidateObject] IReadOnlyCollection<ICodeStatementBase> GetterCodeStatements { get; }
    [Required][ValidateObject] IReadOnlyCollection<ICodeStatementBase> SetterCodeStatements { get; }
    [Required][ValidateObject] IReadOnlyCollection<ICodeStatementBase> InitializerCodeStatements { get; }
}

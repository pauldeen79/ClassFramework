namespace ClassFramework.CodeGeneration.Models;

internal interface IProperty : Abstractions.IMetadataContainer, Abstractions.IExtendedVisibilityContainer, Abstractions.INameContainer, Abstractions.IAttributesContainer, Abstractions.ITypeContainer, Abstractions.IDefaultValueContainer, Abstractions.IExplicitInterfaceNameContainer, Abstractions.IParentTypeContainer
{
    [DefaultValue(true)] bool HasGetter { get; }
    [DefaultValue(true)] bool HasSetter { get; }
    bool HasInitializer { get; }
    SubVisibility GetterVisibility { get; }
    SubVisibility SetterVisibility { get; }
    SubVisibility InitializerVisibility { get; }
    [Required] IReadOnlyCollection<ICodeStatementBase> GetterCodeStatements { get; }
    [Required] IReadOnlyCollection<ICodeStatementBase> SetterCodeStatements { get; }
    [Required] IReadOnlyCollection<ICodeStatementBase> InitializerCodeStatements { get; }
}

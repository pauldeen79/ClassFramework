namespace ClassFramework.CodeGeneration.Bootstrap.Models;

internal interface IProperty : IExtendedVisibilityContainer, INameContainer, IAttributesContainer, ITypeContainer, IDefaultValueContainer, IExplicitInterfaceNameContainer, IParentTypeContainer
{
    bool HasGetter { get; }
    bool HasSetter { get; }
    bool HasInitializer { get; }
    SubVisibility GetterVisibility { get; }
    SubVisibility SetterVisibility { get; }
    SubVisibility InitializerVisibility { get; }
    [Required] IReadOnlyCollection<ICodeStatementBase> GetterCodeStatements { get; }
    [Required] IReadOnlyCollection<ICodeStatementBase> SetterCodeStatements { get; }
    [Required] IReadOnlyCollection<ICodeStatementBase> InitializerCodeStatements { get; }
}

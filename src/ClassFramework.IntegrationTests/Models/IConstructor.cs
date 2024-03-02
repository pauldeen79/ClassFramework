namespace ClassFramework.CodeGeneration.Models;

internal interface IConstructor : Abstractions.IMetadataContainer, Abstractions.IExtendedVisibilityContainer, Abstractions.IAttributesContainer, Abstractions.ICodeStatementsContainer, Abstractions.IParametersContainer, ISuppressWarningCodesContainer
{
    [Required(AllowEmptyStrings = true)] string ChainCall { get; }
}

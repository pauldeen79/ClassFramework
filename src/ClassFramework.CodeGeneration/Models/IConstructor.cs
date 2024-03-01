namespace ClassFramework.CodeGeneration.Models;

internal interface IConstructor : IMetadataContainer, IExtendedVisibilityContainer, IAttributesContainer, ICodeStatementsContainer, IParametersContainer, ISuppressWarningCodesContainer
{
    [Required(AllowEmptyStrings = true)] string ChainCall { get; }
}

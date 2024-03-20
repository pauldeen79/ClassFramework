namespace ClassFramework.CodeGeneration.Bootstrap.Models;

internal interface IConstructor : IExtendedVisibilityContainer, IAttributesContainer, ICodeStatementsContainer, IParametersContainer, ISuppressWarningCodesContainer
{
    [Required(AllowEmptyStrings = true)] string ChainCall { get; }
}

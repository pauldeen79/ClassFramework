namespace ClassFramework.CodeGeneration.Models;

internal interface IConstructor : Abstractions.IModifiersContainer, Abstractions.IAttributesContainer, Abstractions.ICodeStatementsContainer, Abstractions.IParametersContainer, Abstractions.ISuppressWarningCodesContainer
{
    [Required(AllowEmptyStrings = true)] string ChainCall { get; }
}

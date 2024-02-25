namespace ClassFramework.IntegrationTests.Models;

internal interface IConstructor : Abstractions.IMetadataContainer, Abstractions.IExtendedVisibilityContainer, Abstractions.IAttributesContainer, Abstractions.ICodeStatementsContainer, Abstractions.IParametersContainer
{
    [Required(AllowEmptyStrings = true)] string ChainCall { get; }
}

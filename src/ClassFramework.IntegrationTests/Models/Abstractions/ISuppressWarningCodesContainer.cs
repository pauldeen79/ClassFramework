namespace ClassFramework.IntegrationTests.Models.Abstractions;

internal interface ISuppressWarningCodesContainer
{
    [Required] IReadOnlyCollection<string> SuppressWarningCodes { get; }
}

namespace ClassFramework.CodeGeneration.Models.Abstractions;

internal interface ISuppressWarningCodesContainer
{
    [Required] IReadOnlyCollection<string> SuppressWarningCodes { get; }
}

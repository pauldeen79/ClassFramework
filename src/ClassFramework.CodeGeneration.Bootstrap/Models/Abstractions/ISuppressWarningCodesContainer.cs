namespace ClassFramework.CodeGeneration.Bootstrap.Models.Abstractions;

internal interface ISuppressWarningCodesContainer
{
    [Required] IReadOnlyCollection<string> SuppressWarningCodes { get; }
}

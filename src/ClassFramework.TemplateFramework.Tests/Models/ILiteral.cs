namespace ClassFramework.TemplateFramework.Tests.Models;

internal interface ILiteral
{
    [Required(AllowEmptyStrings = true)] string Value { get; }
    object? OriginalValue { get; }
}

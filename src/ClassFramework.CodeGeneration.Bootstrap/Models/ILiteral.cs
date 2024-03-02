namespace ClassFramework.CodeGeneration.Bootstrap.Models;

internal interface ILiteral
{
    [Required(AllowEmptyStrings = true)] string Value { get; }
    object? OriginalValue { get; }
}

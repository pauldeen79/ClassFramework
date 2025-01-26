namespace CrossCutting.CodeGeneration.Models;

internal interface IFormattableStringParserSettings
{
    IFormatProvider FormatProvider { get; }
    [Required(AllowEmptyStrings = false)] string PlaceholderStart { get; }
    [Required(AllowEmptyStrings = false)] string PlaceholderEnd { get; }
    [DefaultValue(true)] bool EscapeBraces { get; }
    [DefaultValue(10)] int MaximumRecursion { get; }
}

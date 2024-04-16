namespace ClassFramework.Pipelines.Abstractions;

public interface IPipelinePlaceholderProcessor
{
    Result<FormattableStringParserResult> Process(string value, IFormatProvider formatProvider, object? context, IFormattableStringParser formattableStringParser);
}

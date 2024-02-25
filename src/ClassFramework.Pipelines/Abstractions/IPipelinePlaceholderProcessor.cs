namespace ClassFramework.Pipelines.Abstractions;

public interface IPipelinePlaceholderProcessor
{
    Result<string> Process(string value, IFormatProvider formatProvider, object? context, IFormattableStringParser formattableStringParser);
}

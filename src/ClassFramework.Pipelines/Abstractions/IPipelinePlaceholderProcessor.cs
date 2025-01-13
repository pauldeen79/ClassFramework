namespace ClassFramework.Pipelines.Abstractions;

public interface IPipelinePlaceholderProcessor
{
    Result<GenericFormattableString> Evaluate(string value, IFormatProvider formatProvider, object? context, IFormattableStringParser formattableStringParser);
}

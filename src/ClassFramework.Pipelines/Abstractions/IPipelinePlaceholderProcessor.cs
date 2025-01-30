namespace ClassFramework.Pipelines.Abstractions;

public interface IPipelinePlaceholderProcessor
{
    Result<GenericFormattableString> Evaluate(string value, PlaceholderSettings settings, object? context, IFormattableStringParser formattableStringParser);
}

namespace ClassFramework.Pipelines.Shared.PlaceholderProcessors;

public class PropertyProcessor : IPipelinePlaceholderProcessor, IPlaceholderProcessor
{
    public int Order => 30;

    public Result<FormattableStringParserResult> Process(string value, IFormatProvider formatProvider, object? context, IFormattableStringParser formattableStringParser)
    {
        if (context is not PropertyContext propertyContext)
        {
            return Result.Continue<FormattableStringParserResult>();
        }

        var typeName = propertyContext.TypeName.FixTypeName();

        return value switch
        {
            _ => Result.Continue<FormattableStringParserResult>()
        };
    }
}

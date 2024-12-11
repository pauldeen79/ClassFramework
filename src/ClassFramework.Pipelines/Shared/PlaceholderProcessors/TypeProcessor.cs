namespace ClassFramework.Pipelines.Shared.PlaceholderProcessors;

public class TypeProcessor : IPipelinePlaceholderProcessor
{
    public Result<FormattableStringParserResult> Process(string value, IFormatProvider formatProvider, object? context, IFormattableStringParser formattableStringParser)
    {
        if (context is not PipelineContext<Type> pipelineContext)
        {
            return Result.Continue<FormattableStringParserResult>();
        }

        return value switch
        {
            $"{nameof(Type.Name)}NoInterfacePrefix" or $"Class.{nameof(Type.Name)}NoInterfacePrefix" => Result.Success<FormattableStringParserResult>(pipelineContext.Request.WithoutInterfacePrefix()),
            "GenericArgumentsWithBrackets" or "Class.GenericArgumentsWithBrackets" => Result.Success<FormattableStringParserResult>(pipelineContext.Request.GetGenericTypeArgumentsString(addBrackets: true)),
            "GenericArgumentsWithoutBrackets" or "Class.GenericArgumentsWithoutBrackets" => Result.Success<FormattableStringParserResult>(pipelineContext.Request.GetGenericTypeArgumentsString(addBrackets: false)),
            _ => Result.Continue<FormattableStringParserResult>()
        };
    }
}

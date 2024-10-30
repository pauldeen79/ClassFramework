namespace ClassFramework.Pipelines.Shared.PlaceholderProcessors;

public class TypeBaseProcessor : IPipelinePlaceholderProcessor
{
    public Result<FormattableStringParserResult> Process(string value, IFormatProvider formatProvider, object? context, IFormattableStringParser formattableStringParser)
    {
        if (context is not PipelineContext<IType> pipelineContext)
        {
            return Result.Continue<FormattableStringParserResult>();
        }

        return value switch
        {
            nameof(IType.Name) or $"Class.{nameof(IType.Name)}" => Result.Success<FormattableStringParserResult>(pipelineContext.Request.Name),
            $"{nameof(IType.Name)}Lower" or $"Class.{nameof(IType.Name)}Lower" => Result.Success<FormattableStringParserResult>(pipelineContext.Request.Name.ToLower(formatProvider.ToCultureInfo())),
            $"{nameof(IType.Name)}Upper" or $"Class.{nameof(IType.Name)}Upper" => Result.Success<FormattableStringParserResult>(pipelineContext.Request.Name.ToUpper(formatProvider.ToCultureInfo())),
            $"{nameof(IType.Name)}Pascal" or $"Class.{nameof(IType.Name)}Pascal" => Result.Success<FormattableStringParserResult>(pipelineContext.Request.Name.ToPascalCase(formatProvider.ToCultureInfo())),
            $"{nameof(IType.Name)}Camel" or $"Class.{nameof(IType.Name)}Camel" => Result.Success<FormattableStringParserResult>(pipelineContext.Request.Name.ToCamelCase(formatProvider.ToCultureInfo())),
            $"{nameof(IType.Namespace)}" or $"Class.{nameof(IType.Namespace)}" => Result.Success<FormattableStringParserResult>(pipelineContext.Request.Namespace),
            $"FullName" or "Class.FullName" => Result.Success<FormattableStringParserResult>(pipelineContext.Request.GetFullName()),
            $"{nameof(IType.Name)}NoInterfacePrefix" or $"Class.{nameof(IType.Name)}NoInterfacePrefix" => Result.Success<FormattableStringParserResult>(pipelineContext.Request.WithoutInterfacePrefix()),
            "GenericArgumentsWithBrackets" or "Class.GenericArgumentsWithBrackets" => Result.Success<FormattableStringParserResult>(pipelineContext.Request.GetGenericTypeArgumentsString(addBrackets: true)),
            "GenericArgumentsWithoutBrackets" or "Class.GenericArgumentsWithoutBrackets" => Result.Success<FormattableStringParserResult>(pipelineContext.Request.GetGenericTypeArgumentsString(addBrackets: false)),
            _ => Result.Continue<FormattableStringParserResult>()
        };
    }
}

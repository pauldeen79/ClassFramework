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
            nameof(IType.Name) or $"Class.{nameof(IType.Name)}" => Result.Success(pipelineContext.Model.Name.ToFormattableStringParserResult()),
            $"{nameof(IType.Name)}Lower" or $"Class.{nameof(IType.Name)}Lower" => Result.Success(pipelineContext.Model.Name.ToLower(formatProvider.ToCultureInfo()).ToFormattableStringParserResult()),
            $"{nameof(IType.Name)}Upper" or $"Class.{nameof(IType.Name)}Upper" => Result.Success(pipelineContext.Model.Name.ToUpper(formatProvider.ToCultureInfo()).ToFormattableStringParserResult()),
            $"{nameof(IType.Name)}Pascal" or $"Class.{nameof(IType.Name)}Pascal" => Result.Success(pipelineContext.Model.Name.ToPascalCase(formatProvider.ToCultureInfo()).ToFormattableStringParserResult()),
            $"{nameof(IType.Namespace)}" or $"Class.{nameof(IType.Namespace)}" => Result.Success(pipelineContext.Model.Namespace.ToFormattableStringParserResult()),
            $"FullName" or "Class.FullName" => Result.Success(pipelineContext.Model.GetFullName().ToFormattableStringParserResult()),
            $"{nameof(IType.Name)}NoInterfacePrefix" or $"Class.{nameof(IType.Name)}NoInterfacePrefix" => Result.Success(pipelineContext.Model.WithoutInterfacePrefix().ToFormattableStringParserResult()),
            "GenericArgumentsWithBrackets" or "Class.GenericArgumentsWithBrackets" => Result.Success(pipelineContext.Model.GetGenericTypeArgumentsString(addBrackets: true).ToFormattableStringParserResult()),
            "GenericArgumentsWithoutBrackets" or "Class.GenericArgumentsWithoutBrackets" => Result.Success(pipelineContext.Model.GetGenericTypeArgumentsString(addBrackets: false).ToFormattableStringParserResult()),
            _ => Result.Continue<FormattableStringParserResult>()
        };
    }
}

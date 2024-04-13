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
            nameof(Type.Name) or $"Class.{nameof(Type.Name)}" => Result.Success(pipelineContext.Model.Name.WithoutGenerics().ToFormattableStringParserResult()),
            $"{nameof(Type.Name)}Lower" or $"Class.{nameof(Type.Name)}Lower" => Result.Success(pipelineContext.Model.Name.WithoutGenerics().ToLower(formatProvider.ToCultureInfo()).ToFormattableStringParserResult()),
            $"{nameof(Type.Name)}Upper" or $"Class.{nameof(Type.Name)}Upper" => Result.Success(pipelineContext.Model.Name.WithoutGenerics().ToUpper(formatProvider.ToCultureInfo()).ToFormattableStringParserResult()),
            $"{nameof(Type.Name)}Pascal" or $"Class.{nameof(Type.Name)}Pascal" => Result.Success(pipelineContext.Model.Name.WithoutGenerics().ToPascalCase(formatProvider.ToCultureInfo()).ToFormattableStringParserResult()),
            $"{nameof(Type.Namespace)}" or $"Class.{nameof(Type.Namespace)}" => Result.Success(pipelineContext.Model.Namespace.ToFormattableStringParserResult()),
            $"{nameof(Type.FullName)}" or $"Class.{nameof(Type.FullName)}" => Result.Success(pipelineContext.Model.FullName.WithoutGenerics().ToFormattableStringParserResult()),
            $"{nameof(Type.Name)}NoInterfacePrefix" or $"Class.{nameof(Type.Name)}NoInterfacePrefix" => Result.Success(pipelineContext.Model.WithoutInterfacePrefix().ToFormattableStringParserResult()),
            "GenericArgumentsWithBrackets" or "Class.GenericArgumentsWithBrackets" => Result.Success(pipelineContext.Model.GetGenericTypeArgumentsString(addBrackets: true).ToFormattableStringParserResult()),
            "GenericArgumentsWithoutBrackets" or "Class.GenericArgumentsWithoutBrackets" => Result.Success(pipelineContext.Model.GetGenericTypeArgumentsString(addBrackets: false).ToFormattableStringParserResult()),
            _ => Result.Continue<FormattableStringParserResult>()
        };
    }
}

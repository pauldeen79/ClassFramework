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
            nameof(Type.Name) or $"Class.{nameof(Type.Name)}" => Result.Success<FormattableStringParserResult>(pipelineContext.Model.Name.WithoutGenerics()),
            $"{nameof(Type.Name)}Lower" or $"Class.{nameof(Type.Name)}Lower" => Result.Success<FormattableStringParserResult>(pipelineContext.Model.Name.WithoutGenerics().ToLower(formatProvider.ToCultureInfo())),
            $"{nameof(Type.Name)}Upper" or $"Class.{nameof(Type.Name)}Upper" => Result.Success<FormattableStringParserResult>(pipelineContext.Model.Name.WithoutGenerics().ToUpper(formatProvider.ToCultureInfo())),
            $"{nameof(Type.Name)}Pascal" or $"Class.{nameof(Type.Name)}Pascal" => Result.Success<FormattableStringParserResult>(pipelineContext.Model.Name.WithoutGenerics().ToPascalCase(formatProvider.ToCultureInfo())),
            $"{nameof(Type.Namespace)}" or $"Class.{nameof(Type.Namespace)}" => Result.Success<FormattableStringParserResult>(pipelineContext.Model.Namespace),
            $"{nameof(Type.FullName)}" or $"Class.{nameof(Type.FullName)}" => Result.Success<FormattableStringParserResult>(pipelineContext.Model.FullName.WithoutGenerics()),
            $"{nameof(Type.Name)}NoInterfacePrefix" or $"Class.{nameof(Type.Name)}NoInterfacePrefix" => Result.Success<FormattableStringParserResult>(pipelineContext.Model.WithoutInterfacePrefix()),
            "GenericArgumentsWithBrackets" or "Class.GenericArgumentsWithBrackets" => Result.Success<FormattableStringParserResult>(pipelineContext.Model.GetGenericTypeArgumentsString(addBrackets: true)),
            "GenericArgumentsWithoutBrackets" or "Class.GenericArgumentsWithoutBrackets" => Result.Success<FormattableStringParserResult>(pipelineContext.Model.GetGenericTypeArgumentsString(addBrackets: false)),
            _ => Result.Continue<FormattableStringParserResult>()
        };
    }
}

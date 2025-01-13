namespace ClassFramework.Pipelines.Interface.PlaceholderProcessors;

public class InterfacePipelinePlaceholderProcessor(IEnumerable<IPipelinePlaceholderProcessor> pipelinePlaceholderProcessors) : IPlaceholder
{
    private readonly IEnumerable<IPipelinePlaceholderProcessor> _pipelinePlaceholderProcessors = pipelinePlaceholderProcessors.IsNotNull(nameof(pipelinePlaceholderProcessors));

    public int Order => 20;

    public Result<GenericFormattableString> Evaluate(string value, IFormatProvider formatProvider, object? context, IFormattableStringParser formattableStringParser)
    {
        formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));

        if (context is PipelineContext<InterfaceContext> pipelineContext)
        {
            return GetResultForPipelineContext(value, formatProvider, formattableStringParser, pipelineContext);
        }

        return Result.Continue<GenericFormattableString>();
    }

    public Result Validate(string value, IFormatProvider formatProvider, object? context, IFormattableStringParser formattableStringParser)
    {
        return Result.Success();
    }

    private Result<GenericFormattableString> GetResultForPipelineContext(string value, IFormatProvider formatProvider, IFormattableStringParser formattableStringParser, PipelineContext<InterfaceContext> pipelineContext)
        => value switch
        {
            _ => _pipelinePlaceholderProcessors.Select(x => x.Evaluate(value, formatProvider, new PipelineContext<IType>(pipelineContext.Request.SourceModel), formattableStringParser)).FirstOrDefault(x => x.Status != ResultStatus.Continue)
                ?? Result.Continue<GenericFormattableString>()
        };
}

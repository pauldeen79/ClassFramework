namespace ClassFramework.Pipelines.Interface.PlaceholderProcessors;

public class InterfacePipelinePlaceholderProcessor : IPlaceholderProcessor
{
    private readonly IEnumerable<IPipelinePlaceholderProcessor> _pipelinePlaceholderProcessors;

    public InterfacePipelinePlaceholderProcessor(IEnumerable<IPipelinePlaceholderProcessor> pipelinePlaceholderProcessors)
    {
        _pipelinePlaceholderProcessors = pipelinePlaceholderProcessors.IsNotNull(nameof(pipelinePlaceholderProcessors));
    }

    public int Order => 20;

    public Result<FormattableStringParserResult> Process(string value, IFormatProvider formatProvider, object? context, IFormattableStringParser formattableStringParser)
    {
        formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));

        if (context is PipelineContext<InterfaceContext, InterfaceBuilder> pipelineContext)
        {
            return GetResultForPipelineContext(value, formatProvider, formattableStringParser, pipelineContext);
        }

        if (context is ParentChildContext<PipelineContext<InterfaceContext, InterfaceBuilder>, Property> parentChildContext)
        {
            return GetResultForParentChildContext(value, formatProvider, formattableStringParser, parentChildContext);
        }

        return Result.Continue<FormattableStringParserResult>();
    }

    private Result<FormattableStringParserResult> GetResultForPipelineContext(string value, IFormatProvider formatProvider, IFormattableStringParser formattableStringParser, PipelineContext<InterfaceContext, InterfaceBuilder> pipelineContext)
        => value switch
        {
            _ => _pipelinePlaceholderProcessors.Select(x => x.Process(value, formatProvider, new PipelineContext<IType>(pipelineContext.Request.SourceModel), formattableStringParser)).FirstOrDefault(x => x.Status != ResultStatus.Continue)
                ?? Result.Continue<FormattableStringParserResult>()
        };

    private Result<FormattableStringParserResult> GetResultForParentChildContext(string value, IFormatProvider formatProvider, IFormattableStringParser formattableStringParser, ParentChildContext<PipelineContext<InterfaceContext, InterfaceBuilder>, Property> parentChildContext)
        => value switch
        {
            _ => _pipelinePlaceholderProcessors.Select(x => x.Process(value, formatProvider, new PropertyContext(parentChildContext.ChildContext, parentChildContext.Settings, formatProvider, parentChildContext.ParentContext.Request.MapTypeName(parentChildContext.ChildContext.TypeName), parentChildContext.Settings.EntityNewCollectionTypeName), formattableStringParser)).FirstOrDefault(x => x.Status != ResultStatus.Continue)
                ?? _pipelinePlaceholderProcessors.Select(x => x.Process(value, formatProvider, new PipelineContext<IType>(parentChildContext.ParentContext.Request.SourceModel), formattableStringParser)).FirstOrDefault(x => x.Status != ResultStatus.Continue)
                ?? Result.Continue<FormattableStringParserResult>()
        };
}

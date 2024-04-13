namespace ClassFramework.Pipelines.Builder.PlaceholderProcessors;

public class BuilderPipelinePlaceholderProcessor : IPlaceholderProcessor
{
    private readonly IEnumerable<IPipelinePlaceholderProcessor> _pipelinePlaceholderProcessors;

    public BuilderPipelinePlaceholderProcessor(IEnumerable<IPipelinePlaceholderProcessor> pipelinePlaceholderProcessors)
    {
        _pipelinePlaceholderProcessors = pipelinePlaceholderProcessors.IsNotNull(nameof(pipelinePlaceholderProcessors));
    }

    public int Order => 20;

    public Result<FormattableStringParserResult> Process(string value, IFormatProvider formatProvider, object? context, IFormattableStringParser formattableStringParser)
    {
        formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));

        if (context is PipelineContext<IConcreteTypeBuilder, BuilderContext> pipelineContext)
        {
            return pipelineContext.Context.GetBuilderPlaceholderProcessorResultForPipelineContext(value, formattableStringParser, pipelineContext, pipelineContext.Context.SourceModel, _pipelinePlaceholderProcessors).Transform(x => x.ToFormattableStringParserResult());
        }

        if (context is ParentChildContext<PipelineContext<IConcreteTypeBuilder, BuilderContext>, Property> parentChildContext)
        {
            if (value == "InstancePrefix")
            {
                return Result.Success(string.Empty.ToFormattableStringParserResult());
            }

            return parentChildContext.ParentContext.Context.GetBuilderPlaceholderProcessorResultForParentChildContext(value, formattableStringParser, parentChildContext.ParentContext.Context, parentChildContext.ParentContext.Context.SourceModel, parentChildContext.ChildContext, parentChildContext.ParentContext.Context.SourceModel, _pipelinePlaceholderProcessors).Transform(x => x.ToFormattableStringParserResult());
        }

        return Result.Continue<FormattableStringParserResult>();
    }
}

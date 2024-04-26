namespace ClassFramework.Pipelines.BuilderExtension.PlaceholderProcessors;

public class BuilderInterfacePipelinePlaceholderProcessor : IPlaceholderProcessor
{
    private readonly IEnumerable<IPipelinePlaceholderProcessor> _pipelinePlaceholderProcessors;

    public BuilderInterfacePipelinePlaceholderProcessor(IEnumerable<IPipelinePlaceholderProcessor> pipelinePlaceholderProcessors)
    {
        _pipelinePlaceholderProcessors = pipelinePlaceholderProcessors.IsNotNull(nameof(pipelinePlaceholderProcessors));
    }

    public int Order => 20;

    public Result<FormattableStringParserResult> Process(string value, IFormatProvider formatProvider, object? context, IFormattableStringParser formattableStringParser)
    {
        formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));

        if (context is PipelineContext<BuilderExtensionContext, IConcreteTypeBuilder> pipelineContext)
        {
            return pipelineContext.Request.GetBuilderPlaceholderProcessorResultForPipelineContext(value, formattableStringParser, pipelineContext.Response, pipelineContext.Request.SourceModel, _pipelinePlaceholderProcessors);
        }

        if (context is ParentChildContext<PipelineContext<BuilderExtensionContext, IConcreteTypeBuilder>, Property> parentChildContext)
        {
            if (value == "InstancePrefix")
            {
                return Result.Success<FormattableStringParserResult>("instance.");
            }

            return parentChildContext.ParentContext.Request.GetBuilderPlaceholderProcessorResultForParentChildContext(value, formattableStringParser, parentChildContext.ParentContext.Request, parentChildContext.ParentContext.Request.SourceModel, parentChildContext.ChildContext, parentChildContext.ParentContext.Request.SourceModel, _pipelinePlaceholderProcessors);
        }

        return Result.Continue<FormattableStringParserResult>();
    }
}

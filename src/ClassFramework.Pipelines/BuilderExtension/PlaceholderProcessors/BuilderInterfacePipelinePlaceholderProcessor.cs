namespace ClassFramework.Pipelines.BuilderExtension.PlaceholderProcessors;

public class BuilderInterfacePipelinePlaceholderProcessor : IPlaceholderProcessor
{
    private readonly IEnumerable<IPipelinePlaceholderProcessor> _pipelinePlaceholderProcessors;

    public BuilderInterfacePipelinePlaceholderProcessor(IEnumerable<IPipelinePlaceholderProcessor> pipelinePlaceholderProcessors)
    {
        _pipelinePlaceholderProcessors = pipelinePlaceholderProcessors.IsNotNull(nameof(pipelinePlaceholderProcessors));
    }

    public int Order => 20;

    public Result<string> Process(string value, IFormatProvider formatProvider, object? context, IFormattableStringParser formattableStringParser)
    {
        formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));

        if (context is PipelineContext<IConcreteTypeBuilder, BuilderExtensionContext> pipelineContext)
        {
            return pipelineContext.Context.GetBuilderPlaceholderProcessorResultForPipelineContext(value, formattableStringParser, pipelineContext.Context, pipelineContext.Context.SourceModel,  _pipelinePlaceholderProcessors);
        }

        if (context is ParentChildContext<PipelineContext<IConcreteTypeBuilder, BuilderExtensionContext>, Property> parentChildContext)
        {
            if (value == "InstancePrefix")
            {
                return Result.Success("instance.");
            }

            return parentChildContext.ParentContext.Context.GetBuilderPlaceholderProcessorResultForParentChildContext(value, formattableStringParser, parentChildContext.ParentContext.Context, parentChildContext.ParentContext.Context.SourceModel, parentChildContext.ChildContext, parentChildContext.ParentContext.Context.SourceModel, _pipelinePlaceholderProcessors);
        }

        return Result.Continue<string>();
    }
}

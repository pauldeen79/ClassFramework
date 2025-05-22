namespace ClassFramework.Pipelines.Builder.PlaceholderProcessors;

public class BuilderPipelinePlaceholderProcessor(IEnumerable<IPipelinePlaceholderProcessor> pipelinePlaceholderProcessors) : IPlaceholder
{
    private readonly IEnumerable<IPipelinePlaceholderProcessor> _pipelinePlaceholderProcessors = pipelinePlaceholderProcessors.IsNotNull(nameof(pipelinePlaceholderProcessors));

    public Result<GenericFormattableString> Evaluate(string value, PlaceholderSettings settings, object? context, IExpressionEvaluator evaluator)
    {
        formattableStringParser = evaluator.IsNotNull(nameof(evaluator));

        if (context is PipelineContext<BuilderContext> pipelineContext)
        {
            return pipelineContext.Request.GetBuilderPlaceholderProcessorResultForPipelineContext(value, formattableStringParser, pipelineContext, pipelineContext.Request.SourceModel, _pipelinePlaceholderProcessors);
        }

        if (context is ParentChildContext<PipelineContext<BuilderContext>, Property> parentChildContext)
        {
            return parentChildContext.ParentContext.Request.GetBuilderPlaceholderProcessorResultForParentChildContext(value, formattableStringParser, parentChildContext.ParentContext.Request, parentChildContext.ParentContext.Request.SourceModel, parentChildContext.ChildContext, parentChildContext.ParentContext.Request.SourceModel, _pipelinePlaceholderProcessors);
        }

        return Result.Continue<GenericFormattableString>();
    }

    public Result Validate(string value, PlaceholderSettings settings, object? context, IExpressionEvaluator evaluator)
    {
        return Result.Success();
    }
}

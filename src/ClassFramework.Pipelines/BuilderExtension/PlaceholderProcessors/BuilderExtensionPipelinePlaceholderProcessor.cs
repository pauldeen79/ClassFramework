//namespace ClassFramework.Pipelines.BuilderExtension.PlaceholderProcessors;

//public class BuilderExtensionPipelinePlaceholderProcessor(IEnumerable<IPipelinePlaceholderProcessor> pipelinePlaceholderProcessors) : IPlaceholder
//{
//    private readonly IEnumerable<IPipelinePlaceholderProcessor> _pipelinePlaceholderProcessors = pipelinePlaceholderProcessors.IsNotNull(nameof(pipelinePlaceholderProcessors));

//    public Result<GenericFormattableString> Evaluate(string value, PlaceholderSettings settings, object? context, IExpressionEvaluator evaluator)
//    {
//        evaluator = evaluator.IsNotNull(nameof(evaluator));

//        if (context is PipelineContext<BuilderExtensionContext> pipelineContext)
//        {
//            return pipelineContext.Request.GetBuilderPlaceholderProcessorResultForPipelineContext(value, evaluator, pipelineContext, pipelineContext.Request.SourceModel, _pipelinePlaceholderProcessors);
//        }

//        if (context is ParentChildContext<PipelineContext<BuilderExtensionContext>, Property> parentChildContext)
//        {
//            return parentChildContext.ParentContext.Request.GetBuilderPlaceholderProcessorResultForParentChildContext(value, evaluator, parentChildContext.ParentContext.Request, parentChildContext.ParentContext.Request.SourceModel, parentChildContext.ChildContext, parentChildContext.ParentContext.Request.SourceModel, _pipelinePlaceholderProcessors);
//        }

//        return Result.Continue<GenericFormattableString>();
//    }

//    public Result Validate(string value, PlaceholderSettings settings, object? context, IExpressionEvaluator evaluator)
//    {
//        return Result.Success();
//    }
//}

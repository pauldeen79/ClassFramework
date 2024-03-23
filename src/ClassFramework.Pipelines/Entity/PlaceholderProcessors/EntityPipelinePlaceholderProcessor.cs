namespace ClassFramework.Pipelines.Entity.PlaceholderProcessors;

public class EntityPipelinePlaceholderProcessor : IPlaceholderProcessor
{
    private readonly IEnumerable<IPipelinePlaceholderProcessor> _pipelinePlaceholderProcessors;

    public EntityPipelinePlaceholderProcessor(IEnumerable<IPipelinePlaceholderProcessor> pipelinePlaceholderProcessors)
    {
        _pipelinePlaceholderProcessors = pipelinePlaceholderProcessors.IsNotNull(nameof(pipelinePlaceholderProcessors));
    }

    public int Order => 10;

    public Result<string> Process(string value, IFormatProvider formatProvider, object? context, IFormattableStringParser formattableStringParser)
    {
        formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));

        if (context is PipelineContext<IConcreteTypeBuilder, EntityContext> pipelineContext)
        {
            return GetResultForPipelineContext(value, formatProvider, formattableStringParser, pipelineContext);
        }

        if (context is ParentChildContext<PipelineContext<IConcreteTypeBuilder, EntityContext>, Property> parentChildContext)
        {
            return GetResultForParentChildContext(value, formatProvider, formattableStringParser, parentChildContext);
        }

        return Result.Continue<string>();
    }

    private Result<string> GetResultForPipelineContext(
        string value,
        IFormatProvider formatProvider,
        IFormattableStringParser formattableStringParser,
        PipelineContext<IConcreteTypeBuilder, EntityContext> pipelineContext)
        => value switch
        {
            "EntityNamespace" => formattableStringParser.Parse(pipelineContext.Context.Settings.EntityNamespaceFormatString, pipelineContext.Context.FormatProvider, pipelineContext.Context),
            _ => _pipelinePlaceholderProcessors.Select(x => x.Process(value, formatProvider, new PipelineContext<IType>(pipelineContext.Context.SourceModel), formattableStringParser)).FirstOrDefault(x => x.Status != ResultStatus.Continue)
                ?? Result.Continue<string>()
        };

    private Result<string> GetResultForParentChildContext(
        string value,
        IFormatProvider formatProvider,
        IFormattableStringParser formattableStringParser,
        ParentChildContext<PipelineContext<IConcreteTypeBuilder, EntityContext>, Property> parentChildContext)
        => value switch
        {
            "EntityNamespace" => formattableStringParser.Parse(parentChildContext.ParentContext.Context.Settings.EntityNamespaceFormatString, parentChildContext.ParentContext.Context.FormatProvider, parentChildContext.ParentContext.Context),
            "NullableRequiredSuffix" => Result.Success(!parentChildContext.ParentContext.Context.Settings.AddNullChecks && !parentChildContext.ChildContext.IsValueType && parentChildContext.ChildContext.IsNullable && parentChildContext.ParentContext.Context.Settings.EnableNullableReferenceTypes
                ? "!"
                : string.Empty),
            _ => _pipelinePlaceholderProcessors.Select(x => x.Process(value, formatProvider, new PropertyContext(parentChildContext.ChildContext, parentChildContext.Settings, formatProvider, parentChildContext.ParentContext.Context.MapTypeName(parentChildContext.ChildContext.TypeName), parentChildContext.Settings.EntityNewCollectionTypeName), formattableStringParser)).FirstOrDefault(x => x.Status != ResultStatus.Continue)
                ?? _pipelinePlaceholderProcessors.Select(x => x.Process(value, formatProvider, new PipelineContext<IType>(parentChildContext.ParentContext.Context.SourceModel), formattableStringParser)).FirstOrDefault(x => x.Status != ResultStatus.Continue)
                ?? Result.Continue<string>()
        };
}

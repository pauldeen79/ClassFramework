﻿namespace ClassFramework.Pipelines.Entity.PlaceholderProcessors;

public class EntityPipelinePlaceholderProcessor(IEnumerable<IPipelinePlaceholderProcessor> pipelinePlaceholderProcessors) : IPlaceholderProcessor
{
    private readonly IEnumerable<IPipelinePlaceholderProcessor> _pipelinePlaceholderProcessors = pipelinePlaceholderProcessors.IsNotNull(nameof(pipelinePlaceholderProcessors));

    public int Order => 10;

    public Result<FormattableStringParserResult> Process(string value, IFormatProvider formatProvider, object? context, IFormattableStringParser formattableStringParser)
    {
        formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));

        if (context is PipelineContext<EntityContext> pipelineContext)
        {
            return GetResultForPipelineContext(value, formatProvider, formattableStringParser, pipelineContext);
        }

        if (context is ParentChildContext<PipelineContext<EntityContext>, Property> parentChildContext)
        {
            return GetResultForParentChildContext(value, formatProvider, formattableStringParser, parentChildContext);
        }

        return Result.Continue<FormattableStringParserResult>();
    }

    private Result<FormattableStringParserResult> GetResultForPipelineContext(
        string value,
        IFormatProvider formatProvider,
        IFormattableStringParser formattableStringParser,
        PipelineContext<EntityContext> pipelineContext)
        => value switch
        {
            _ => _pipelinePlaceholderProcessors.Select(x => x.Process(value, formatProvider, new PipelineContext<IType>(pipelineContext.Request.SourceModel), formattableStringParser)).FirstOrDefault(x => x.Status != ResultStatus.Continue)
                ?? Result.Continue<FormattableStringParserResult>()
        };

    private Result<FormattableStringParserResult> GetResultForParentChildContext(
        string value,
        IFormatProvider formatProvider,
        IFormattableStringParser formattableStringParser,
        ParentChildContext<PipelineContext<EntityContext>, Property> parentChildContext)
        => value switch
        {
            _ => _pipelinePlaceholderProcessors.Select(x => x.Process(value, formatProvider, new PropertyContext(parentChildContext.ChildContext, parentChildContext.Settings, formatProvider, parentChildContext.ParentContext.Request.MapTypeName(parentChildContext.ChildContext.TypeName), parentChildContext.Settings.EntityNewCollectionTypeName), formattableStringParser)).FirstOrDefault(x => x.Status != ResultStatus.Continue)
                ?? _pipelinePlaceholderProcessors.Select(x => x.Process(value, formatProvider, new PipelineContext<IType>(parentChildContext.ParentContext.Request.SourceModel), formattableStringParser)).FirstOrDefault(x => x.Status != ResultStatus.Continue)
                ?? Result.Continue<FormattableStringParserResult>()
        };
}

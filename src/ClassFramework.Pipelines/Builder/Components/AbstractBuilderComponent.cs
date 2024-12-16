namespace ClassFramework.Pipelines.Builder.Components;

public class AbstractBuilderComponentBuilder(IFormattableStringParser formattableStringParser) : IBuilderComponentBuilder
{
    private readonly IFormattableStringParser _formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));

    public IPipelineComponent<BuilderContext> Build()
        => new AbstractBuilderComponent(_formattableStringParser);
}

public class AbstractBuilderComponent(IFormattableStringParser formattableStringParser) : IPipelineComponent<BuilderContext>
{
    private readonly IFormattableStringParser _formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));

    public Task<Result> Process(PipelineContext<BuilderContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        if (context.Request.IsBuilderForAbstractEntity /*&& context.Request.IsAbstractBuilder*/)
        {
            var nameResult = _formattableStringParser.Parse(context.Request.Settings.BuilderNameFormatString, context.Request.FormatProvider, context.Request);
            if (!nameResult.IsSuccessful())
            {
                return Task.FromResult<Result>(nameResult);
            }

            context.Request.Builder.WithAbstract();

            if (!context.Request.Settings.IsForAbstractBuilder)
            {
                context.Request.Builder
                    .AddGenericTypeArguments("TBuilder", "TEntity")
                    .AddGenericTypeArgumentConstraints($"where TEntity : {context.Request.SourceModel.GetFullName()}")
                    .AddGenericTypeArgumentConstraints($"where TBuilder : {nameResult.Value}<TBuilder, TEntity>")
                    .WithAbstract();
            }
        }

        return Task.FromResult(Result.Continue());
    }
}

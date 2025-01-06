namespace ClassFramework.Pipelines.Builder.Components;

public class SetNameComponentBuilder(IFormattableStringParser formattableStringParser) : IBuilderComponentBuilder
{
    private readonly IFormattableStringParser _formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));

    public IPipelineComponent<BuilderContext> Build()
        => new SetNameComponent(_formattableStringParser);
}

public class SetNameComponent(IFormattableStringParser formattableStringParser) : IPipelineComponent<BuilderContext>
{
    private readonly IFormattableStringParser _formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));

    public Task<Result> Process(PipelineContext<BuilderContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        var results = new ResultDictionaryBuilder<FormattableStringParserResult>()
            .Add(NamedResults.Name, () => _formattableStringParser.Parse(context.Request.Settings.BuilderNameFormatString, context.Request.FormatProvider, context.Request))
            .Add(NamedResults.Namespace, () => context.Request.GetMappingMetadata(context.Request.SourceModel.GetFullName()).GetFormattableStringParserResult(MetadataNames.CustomBuilderNamespace, () => _formattableStringParser.Parse(context.Request.Settings.BuilderNamespaceFormatString, context.Request.FormatProvider, context.Request)))
            .Build();

        var error = results.GetError();
        if (error is not null)
        {
            // Error in formattable string parsing
            return Task.FromResult<Result>(error);
        }

        context.Request.Builder
            .WithName(results[NamedResults.Name].Value!)
            .WithNamespace(results[NamedResults.Namespace].Value!);

        return Task.FromResult(Result.Continue());
    }
}

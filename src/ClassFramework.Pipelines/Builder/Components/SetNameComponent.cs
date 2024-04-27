namespace ClassFramework.Pipelines.Builder.Components;

public class SetNameComponentBuilder : IBuilderComponentBuilder
{
    private readonly IFormattableStringParser _formattableStringParser;

    public SetNameComponentBuilder(IFormattableStringParser formattableStringParser)
    {
        _formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));
    }

    public IPipelineComponent<BuilderContext, IConcreteTypeBuilder> Build()
        => new SetNameComponent(_formattableStringParser);
}

public class SetNameComponent : IPipelineComponent<BuilderContext, IConcreteTypeBuilder>
{
    private readonly IFormattableStringParser _formattableStringParser;

    public SetNameComponent(IFormattableStringParser formattableStringParser)
    {
        _formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));
    }

    public Task<Result> Process(PipelineContext<BuilderContext, IConcreteTypeBuilder> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        var resultSetBuilder = new NamedResultSetBuilder<FormattableStringParserResult>();
        resultSetBuilder.Add(NamedResults.Name, () => _formattableStringParser.Parse(context.Request.Settings.BuilderNameFormatString, context.Request.FormatProvider, context));
        resultSetBuilder.Add(NamedResults.Namespace, () => context.Request.GetMappingMetadata(context.Request.SourceModel.GetFullName()).GetFormattableStringParserResult(MetadataNames.CustomBuilderNamespace, () => _formattableStringParser.Parse(context.Request.Settings.BuilderNamespaceFormatString, context.Request.FormatProvider, context)));
        var results = resultSetBuilder.Build();
        
        var error = Array.Find(results, x => !x.Result.IsSuccessful());
        if (error is not null)
        {
            // Error in formattable string parsing
            return Task.FromResult<Result>(error.Result);
        }

        context.Response
            .WithName(results.First(x => x.Name == NamedResults.Name).Result.Value!)
            .WithNamespace(results.First(x => x.Name == NamedResults.Namespace).Result.Value!);

        return Task.FromResult(Result.Continue());
    }
}

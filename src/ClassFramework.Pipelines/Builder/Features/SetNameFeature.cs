namespace ClassFramework.Pipelines.Builder.Features;

public class SetNameFeatureBuilder : IBuilderFeatureBuilder
{
    private readonly IFormattableStringParser _formattableStringParser;

    public SetNameFeatureBuilder(IFormattableStringParser formattableStringParser)
    {
        _formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));
    }

    public IPipelineFeature<IConcreteTypeBuilder, BuilderContext> Build()
        => new SetNameFeature(_formattableStringParser);
}

public class SetNameFeature : IPipelineFeature<IConcreteTypeBuilder, BuilderContext>
{
    private readonly IFormattableStringParser _formattableStringParser;

    public SetNameFeature(IFormattableStringParser formattableStringParser)
    {
        _formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));
    }

    public Result<IConcreteTypeBuilder> Process(PipelineContext<IConcreteTypeBuilder, BuilderContext> context)
    {
        context = context.IsNotNull(nameof(context));

        var resultSetBuilder = new NamedResultSetBuilder<string>();
        resultSetBuilder.Add(NamedResults.Name, () => _formattableStringParser.Parse(context.Context.Settings.BuilderNameFormatString, context.Context.FormatProvider, context));
        resultSetBuilder.Add(NamedResults.Namespace, () => context.Context.GetMappingMetadata(context.Context.SourceModel.GetFullName()).GetStringResult(MetadataNames.CustomBuilderNamespace, () => _formattableStringParser.Parse(context.Context.Settings.BuilderNamespaceFormatString, context.Context.FormatProvider, context)));
        var results = resultSetBuilder.Build();
        
        var error = Array.Find(results, x => !x.Result.IsSuccessful());
        if (error is not null)
        {
            // Error in formattable string parsing
            return Result.FromExistingResult<IConcreteTypeBuilder>(error.Result);
        }

        context.Model
            .WithName(results.First(x => x.Name == NamedResults.Name).Result.Value!)
            .WithNamespace(results.First(x => x.Name == NamedResults.Namespace).Result.Value!);

        return Result.Continue<IConcreteTypeBuilder>();
    }

    public IBuilder<IPipelineFeature<IConcreteTypeBuilder, BuilderContext>> ToBuilder()
        => new SetNameFeatureBuilder(_formattableStringParser);
}

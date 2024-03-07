namespace ClassFramework.Pipelines.BuilderExtension.Features;

public class SetNameFeatureBuilder : IBuilderInterfaceFeatureBuilder
{
    private readonly IFormattableStringParser _formattableStringParser;

    public SetNameFeatureBuilder(IFormattableStringParser formattableStringParser)
    {
        _formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));
    }

    public IPipelineFeature<IConcreteTypeBuilder, BuilderExtensionContext> Build()
        => new SetNameFeature(_formattableStringParser);
}

public class SetNameFeature : IPipelineFeature<IConcreteTypeBuilder, BuilderExtensionContext>
{
    private readonly IFormattableStringParser _formattableStringParser;

    public SetNameFeature(IFormattableStringParser formattableStringParser)
    {
        _formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));
    }

    public Result<IConcreteTypeBuilder> Process(PipelineContext<IConcreteTypeBuilder, BuilderExtensionContext> context)
    {
        context = context.IsNotNull(nameof(context));

        var resultSetBuilder = new NamedResultSetBuilder<string>();
        resultSetBuilder.Add("Name", () => _formattableStringParser.Parse(context.Context.Settings.BuilderExtensionsNameFormatString, context.Context.FormatProvider, context));
        resultSetBuilder.Add("Namespace", () => _formattableStringParser.Parse(context.Context.Settings.BuilderExtensionsNamespaceFormatString, context.Context.FormatProvider, context));
        var results = resultSetBuilder.Build();

        var error = Array.Find(results, x => !x.Result.IsSuccessful());
        if (error is not null)
        {
            // Error in formattable string parsing
            return Result.FromExistingResult<IConcreteTypeBuilder>(error.Result);
        }

        context.Model
            .WithName(results.First(x => x.Name == "Name").Result.Value!)
            .WithNamespace(results.First(x => x.Name == "Namespace").Result.Value!);

        return Result.Continue<IConcreteTypeBuilder>();
    }

    public IBuilder<IPipelineFeature<IConcreteTypeBuilder, BuilderExtensionContext>> ToBuilder()
        => new SetNameFeatureBuilder(_formattableStringParser);
}

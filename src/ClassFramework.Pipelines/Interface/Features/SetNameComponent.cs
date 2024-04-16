namespace ClassFramework.Pipelines.Interface.Features;

public class SetNameComponentBuilder : IInterfaceComponentBuilder
{
    private readonly IFormattableStringParser _formattableStringParser;

    public SetNameComponentBuilder(IFormattableStringParser formattableStringParser)
    {
        _formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));
    }

    public IPipelineComponent<InterfaceBuilder, InterfaceContext> Build()
        => new SetNameComponent(_formattableStringParser);
}

public class SetNameComponent : IPipelineComponent<InterfaceBuilder, InterfaceContext>
{
    private readonly IFormattableStringParser _formattableStringParser;

    public SetNameComponent(IFormattableStringParser formattableStringParser)
    {
        _formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));
    }

    public Task<Result<InterfaceBuilder>> Process(PipelineContext<InterfaceBuilder, InterfaceContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        var resultSetBuilder = new NamedResultSetBuilder<FormattableStringParserResult>();
        resultSetBuilder.Add(NamedResults.Name, () => _formattableStringParser.Parse(context.Context.Settings.NameFormatString, context.Context.FormatProvider, context));
        resultSetBuilder.Add(NamedResults.Namespace, () => context.Context.GetMappingMetadata(context.Context.SourceModel.GetFullName()).GetFormattableStringParserResult(MetadataNames.CustomEntityNamespace, () => _formattableStringParser.Parse(context.Context.Settings.NamespaceFormatString, context.Context.FormatProvider, context)));
        var results = resultSetBuilder.Build();

        var error = Array.Find(results, x => !x.Result.IsSuccessful());
        if (error is not null)
        {
            // Error in formattable string parsing
            return Task.FromResult(Result.FromExistingResult<InterfaceBuilder>(error.Result));
        }

        context.Model
            .WithName(results.First(x => x.Name == NamedResults.Name).Result.Value!)
            .WithNamespace(context.Context.MapNamespace(results.First(x => x.Name == NamedResults.Namespace).Result.Value!));

        return Task.FromResult(Result.Continue<InterfaceBuilder>());
    }
}

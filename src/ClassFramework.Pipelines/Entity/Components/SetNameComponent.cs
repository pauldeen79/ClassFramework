namespace ClassFramework.Pipelines.Entity.Features;

public class SetNameComponentBuilder : IEntityComponentBuilder
{
    private readonly IFormattableStringParser _formattableStringParser;

    public SetNameComponentBuilder(IFormattableStringParser formattableStringParser)
    {
        _formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));
    }

    public IPipelineComponent<IConcreteTypeBuilder, EntityContext> Build()
        => new SetNameComponent(_formattableStringParser);
}

public class SetNameComponent : IPipelineComponent<IConcreteTypeBuilder, EntityContext>
{
    private readonly IFormattableStringParser _formattableStringParser;

    public SetNameComponent(IFormattableStringParser formattableStringParser)
    {
        _formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));
    }

    public Result<IConcreteTypeBuilder> Process(PipelineContext<IConcreteTypeBuilder, EntityContext> context)
    {
        context = context.IsNotNull(nameof(context));

        var resultSetBuilder = new NamedResultSetBuilder<string>();
        resultSetBuilder.Add(NamedResults.Name, () => _formattableStringParser.Parse(context.Context.Settings.EntityNameFormatString, context.Context.FormatProvider, context));
        resultSetBuilder.Add(NamedResults.Namespace, () => context.Context.GetMappingMetadata(context.Context.SourceModel.GetFullName()).GetStringResult(MetadataNames.CustomEntityNamespace, () => _formattableStringParser.Parse(context.Context.Settings.EntityNamespaceFormatString, context.Context.FormatProvider, context)));
        var results = resultSetBuilder.Build();

        var error = Array.Find(results, x => !x.Result.IsSuccessful());
        if (error is not null)
        {
            // Error in formattable string parsing
            return Result.FromExistingResult<IConcreteTypeBuilder>(error.Result);
        }

        context.Model
            .WithName(results.First(x => x.Name == NamedResults.Name).Result.Value!)
            .WithNamespace(context.Context.MapNamespace(results.First(x => x.Name == NamedResults.Namespace).Result.Value!));

        return Result.Continue<IConcreteTypeBuilder>();
    }
}

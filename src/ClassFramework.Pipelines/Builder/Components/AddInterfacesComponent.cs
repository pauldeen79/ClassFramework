namespace ClassFramework.Pipelines.Builder.Components;

public class AddInterfacesComponentBuilder : IBuilderComponentBuilder
{
    private readonly IFormattableStringParser _formattableStringParser;

    public AddInterfacesComponentBuilder(IFormattableStringParser formattableStringParser)
    {
        _formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));
    }

    public IPipelineComponent<BuilderContext, IConcreteTypeBuilder> Build()
        => new AddInterfacesComponent(_formattableStringParser);
}

public class AddInterfacesComponent : IPipelineComponent<BuilderContext, IConcreteTypeBuilder>
{
    private readonly IFormattableStringParser _formattableStringParser;

    public AddInterfacesComponent(IFormattableStringParser formattableStringParser)
    {
        _formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));
    }

    public Task<Result> Process(PipelineContext<BuilderContext, IConcreteTypeBuilder> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        if (!context.Request.Settings.CopyInterfaces)
        {
            return Task.FromResult(Result.Continue());
        }

        var results = context.Request.SourceModel.Interfaces
            .Where(x => context.Request.Settings.CopyInterfacePredicate?.Invoke(x) ?? true)
            .Select(x =>
            {
                var metadata = context.Request.GetMappingMetadata(x);
                var ns = metadata.GetStringValue(MetadataNames.CustomBuilderInterfaceNamespace);

                if (!string.IsNullOrEmpty(ns))
                {
                    var property = new PropertyBuilder().WithName("Dummy").WithTypeName(x).Build();
                    var newTypeName = metadata.GetStringValue(MetadataNames.CustomBuilderInterfaceName, "{TypeName}");
                    var newFullName = $"{ns}.{newTypeName}";

                    return _formattableStringParser.Parse
                    (
                        newFullName,
                        context.Request.FormatProvider,
                        new ParentChildContext<PipelineContext<BuilderContext, IConcreteTypeBuilder>, Property>(context, property, context.Request.Settings)
                    ).TransformValue(x => x.ToString());
                }
                return Result.Success(context.Request.MapTypeName(x.FixTypeName()));
            })
            .TakeWhileWithFirstNonMatching(x => x.IsSuccessful())
            .ToArray();

        var error = Array.Find(results, x => !x.IsSuccessful());
        if (error is not null)
        {
            return Task.FromResult<Result>(error);
        }

        context.Response.AddInterfaces(results.Where(x => !string.IsNullOrEmpty(x.Value)).Select(x => x.Value!));

        return Task.FromResult(Result.Continue());
    }
}

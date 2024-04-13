namespace ClassFramework.Pipelines.Builder.Features;

public class AddInterfacesComponentBuilder : IBuilderComponentBuilder
{
    private readonly IFormattableStringParser _formattableStringParser;

    public AddInterfacesComponentBuilder(IFormattableStringParser formattableStringParser)
    {
        _formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));
    }

    public IPipelineComponent<IConcreteTypeBuilder, BuilderContext> Build()
        => new AddInterfacesComponent(_formattableStringParser);
}

public class AddInterfacesComponent : IPipelineComponent<IConcreteTypeBuilder, BuilderContext>
{
    private readonly IFormattableStringParser _formattableStringParser;

    public AddInterfacesComponent(IFormattableStringParser formattableStringParser)
    {
        _formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));
    }

    public Task<Result<IConcreteTypeBuilder>> Process(PipelineContext<IConcreteTypeBuilder, BuilderContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        if (!context.Context.Settings.CopyInterfaces)
        {
            return Task.FromResult(Result.Continue<IConcreteTypeBuilder>());
        }

        var results = context.Context.SourceModel.Interfaces
            .Where(x => context.Context.Settings.CopyInterfacePredicate?.Invoke(x) ?? true)
            .Select(x =>
            {
                var metadata = context.Context.GetMappingMetadata(x);
                var ns = metadata.GetStringValue(MetadataNames.CustomBuilderInterfaceNamespace);

                if (!string.IsNullOrEmpty(ns))
                {
                    var property = new PropertyBuilder().WithName("Dummy").WithTypeName(x).Build();
                    var newTypeName = metadata.GetStringValue(MetadataNames.CustomBuilderInterfaceName, "{TypeName}");
                    var newFullName = $"{ns}.{newTypeName}";

                    return _formattableStringParser.Parse
                    (
                        newFullName,
                        context.Context.FormatProvider,
                        new ParentChildContext<PipelineContext<IConcreteTypeBuilder, BuilderContext>, Property>(context, property, context.Context.Settings)
                    );
                }
                return Result.Success(context.Context.MapTypeName(x.FixTypeName()));
            })
            .TakeWhileWithFirstNonMatching(x => x.IsSuccessful())
            .ToArray();

        var error = Array.Find(results, x => !x.IsSuccessful());
        if (error is not null)
        {
            return Result.FromExistingResult<IConcreteTypeBuilder>(error);
        }

        context.Model.AddInterfaces(results.Where(x => !string.IsNullOrEmpty(x.Value)).Select(x => x.Value!));

        return Task.FromResult(Result.Continue<IConcreteTypeBuilder>());
    }
}

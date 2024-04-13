namespace ClassFramework.Pipelines.Entity.Features;

public class SetBaseClassComponentBuilder : IEntityComponentBuilder
{
    private readonly IFormattableStringParser _formattableStringParser;

    public SetBaseClassComponentBuilder(IFormattableStringParser formattableStringParser)
    {
        _formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));
    }

    public IPipelineComponent<IConcreteTypeBuilder, EntityContext> Build()
        => new SetBaseClassComponent(_formattableStringParser);
}

public class SetBaseClassComponent : IPipelineComponent<IConcreteTypeBuilder, EntityContext>
{
    private readonly IFormattableStringParser _formattableStringParser;

    public SetBaseClassComponent(IFormattableStringParser formattableStringParser)
    {
        _formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));
    }

    public Task<Result<IConcreteTypeBuilder>> Process(PipelineContext<IConcreteTypeBuilder, EntityContext> context)
    {
        context = context.IsNotNull(nameof(context));

        if (context.Model is IBaseClassContainerBuilder baseClassContainerBuilder)
        {
            baseClassContainerBuilder.WithBaseClass(context.Context.SourceModel.GetEntityBaseClass(context.Context.Settings.EnableInheritance, context.Context.Settings.BaseClass));
        }

        return Task.FromResult(Result.Continue<IConcreteTypeBuilder>());
    }
}

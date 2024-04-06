namespace ClassFramework.Pipelines.Entity.Features;

public class PartialComponentBuilder : IEntityComponentBuilder
{
    public IPipelineComponent<IConcreteTypeBuilder, EntityContext> Build() => new PartialComponent();
}

public class PartialComponent : IPipelineComponent<IConcreteTypeBuilder, EntityContext>
{
    public Result<IConcreteTypeBuilder> Process(PipelineContext<IConcreteTypeBuilder, EntityContext> context)
    {
        context = context.IsNotNull(nameof(context));

        context.Model.WithPartial(context.Context.Settings.CreateAsPartial);

        return Result.Continue<IConcreteTypeBuilder>();
    }
}

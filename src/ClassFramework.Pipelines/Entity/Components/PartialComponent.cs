namespace ClassFramework.Pipelines.Entity.Features;

public class PartialComponentBuilder : IEntityComponentBuilder
{
    public IPipelineComponent<IConcreteTypeBuilder, EntityContext> Build() => new PartialComponent();
}

public class PartialComponent : IPipelineComponent<IConcreteTypeBuilder, EntityContext>
{
    public Task<Result<IConcreteTypeBuilder>> Process(PipelineContext<IConcreteTypeBuilder, EntityContext> context)
    {
        context = context.IsNotNull(nameof(context));

        context.Model.WithPartial(context.Context.Settings.CreateAsPartial);

        return Task.FromResult(Result.Continue<IConcreteTypeBuilder>());
    }
}

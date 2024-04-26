namespace ClassFramework.Pipelines.Entity.Features;

public class PartialComponentBuilder : IEntityComponentBuilder
{
    public IPipelineComponent<EntityContext, IConcreteTypeBuilder> Build() => new PartialComponent();
}

public class PartialComponent : IPipelineComponent<EntityContext, IConcreteTypeBuilder>
{
    public Task<Result> Process(PipelineContext<EntityContext, IConcreteTypeBuilder> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        context.Response.WithPartial(context.Request.Settings.CreateAsPartial);

        return Task.FromResult(Result.Continue());
    }
}

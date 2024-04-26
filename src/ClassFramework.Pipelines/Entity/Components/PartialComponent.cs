namespace ClassFramework.Pipelines.Entity.Features;

public class PartialComponentBuilder : IEntityComponentBuilder
{
    public IPipelineComponent<IConcreteTypeBuilder, EntityContext> Build() => new PartialComponent();
}

public class PartialComponent : IPipelineComponent<IConcreteTypeBuilder, EntityContext>
{
    public Task<Result<IConcreteTypeBuilder>> Process(PipelineContext<IConcreteTypeBuilder, EntityContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        context.Response.WithPartial(context.Request.Settings.CreateAsPartial);

        return Task.FromResult(Result.Continue<IConcreteTypeBuilder>());
    }
}

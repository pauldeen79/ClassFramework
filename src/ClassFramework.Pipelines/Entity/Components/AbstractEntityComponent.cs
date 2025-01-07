namespace ClassFramework.Pipelines.Entity.Components;

public class AbstractEntityComponentBuilder : IEntityComponentBuilder
{
    public IPipelineComponent<EntityContext> Build()
        => new AbstractEntityComponent();
}

public class AbstractEntityComponent : IPipelineComponent<EntityContext>
{
    public Task<Result> Process(PipelineContext<EntityContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        context.Request.Builder.WithAbstract(context.Request.IsAbstract);

        return Task.FromResult(Result.Success());
    }
}

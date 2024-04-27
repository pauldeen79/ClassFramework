namespace ClassFramework.Pipelines.Entity.Components;

public class AbstractEntityComponentBuilder : IEntityComponentBuilder
{
    public IPipelineComponent<EntityContext, IConcreteTypeBuilder> Build()
        => new AbstractEntityComponent();
}

public class AbstractEntityComponent : IPipelineComponent<EntityContext, IConcreteTypeBuilder>
{
    public Task<Result> Process(PipelineContext<EntityContext, IConcreteTypeBuilder> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        if (context.Response is ClassBuilder cls)
        {
            cls.WithAbstract(context.Request.IsAbstract);
        }

        return Task.FromResult(Result.Continue());
    }
}

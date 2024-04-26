namespace ClassFramework.Pipelines.Entity.Features;

public class AbstractEntityComponentBuilder : IEntityComponentBuilder
{
    public IPipelineComponent<IConcreteTypeBuilder, EntityContext> Build()
        => new AbstractEntityComponent();
}

public class AbstractEntityComponent : IPipelineComponent<IConcreteTypeBuilder, EntityContext>
{
    public Task<Result<IConcreteTypeBuilder>> Process(PipelineContext<IConcreteTypeBuilder, EntityContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        if (context.Model is ClassBuilder cls)
        {
            cls.WithAbstract(context.Request.IsAbstract);
        }

        return Task.FromResult(Result.Continue<IConcreteTypeBuilder>());
    }
}

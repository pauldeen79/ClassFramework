namespace ClassFramework.Pipelines.Entity.Features;

public class SetBaseClassComponentBuilder : IEntityComponentBuilder
{
    public IPipelineComponent<EntityContext, IConcreteTypeBuilder> Build()
        => new SetBaseClassComponent();
}

public class SetBaseClassComponent : IPipelineComponent<EntityContext, IConcreteTypeBuilder>
{
    public Task<Result> Process(PipelineContext<EntityContext, IConcreteTypeBuilder> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        if (context.Response is IBaseClassContainerBuilder baseClassContainerBuilder)
        {
            baseClassContainerBuilder.WithBaseClass(context.Request.SourceModel.GetEntityBaseClass(context.Request.Settings.EnableInheritance, context.Request.Settings.BaseClass));
        }

        return Task.FromResult(Result.Continue());
    }
}

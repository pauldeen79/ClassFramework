namespace ClassFramework.Pipelines.Entity.Features;

public class SetBaseClassComponentBuilder : IEntityComponentBuilder
{
    public IPipelineComponent<IConcreteTypeBuilder, EntityContext> Build()
        => new SetBaseClassComponent();
}

public class SetBaseClassComponent : IPipelineComponent<IConcreteTypeBuilder, EntityContext>
{
    public Task<Result<IConcreteTypeBuilder>> Process(PipelineContext<IConcreteTypeBuilder, EntityContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        if (context.Model is IBaseClassContainerBuilder baseClassContainerBuilder)
        {
            baseClassContainerBuilder.WithBaseClass(context.Context.SourceModel.GetEntityBaseClass(context.Context.Settings.EnableInheritance, context.Context.Settings.BaseClass));
        }

        return Task.FromResult(Result.Continue<IConcreteTypeBuilder>());
    }
}

namespace ClassFramework.Pipelines.Entity.Components;

public class AddGenericsComponentBuilder : IEntityComponentBuilder
{
    public IPipelineComponent<EntityContext, IConcreteTypeBuilder> Build()
        => new AddGenericsComponent();
}

public class AddGenericsComponent : IPipelineComponent<EntityContext, IConcreteTypeBuilder>
{
    public Task<Result> Process(PipelineContext<EntityContext, IConcreteTypeBuilder> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        context.Response
            .AddGenericTypeArguments(context.Request.SourceModel.GenericTypeArguments)
            .AddGenericTypeArgumentConstraints(context.Request.SourceModel.GenericTypeArgumentConstraints);

        return Task.FromResult(Result.Continue());
    }
}

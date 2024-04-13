namespace ClassFramework.Pipelines.Entity.Features;

public class AddGenericsComponentBuilder : IEntityComponentBuilder
{
    public IPipelineComponent<IConcreteTypeBuilder, EntityContext> Build()
        => new AddGenericsComponent();
}

public class AddGenericsComponent : IPipelineComponent<IConcreteTypeBuilder, EntityContext>
{
    public Task<Result<IConcreteTypeBuilder>> Process(PipelineContext<IConcreteTypeBuilder, EntityContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        context.Model
            .AddGenericTypeArguments(context.Context.SourceModel.GenericTypeArguments)
            .AddGenericTypeArgumentConstraints(context.Context.SourceModel.GenericTypeArgumentConstraints);

        return Task.FromResult(Result.Continue<IConcreteTypeBuilder>());
    }
}

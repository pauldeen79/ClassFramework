namespace ClassFramework.Pipelines.Builder.Features;

public class GenericsComponentBuilder : IBuilderComponentBuilder
{
    public IPipelineComponent<IConcreteTypeBuilder, BuilderContext> Build()
        => new GenericsComponent();
}

public class GenericsComponent : IPipelineComponent<IConcreteTypeBuilder, BuilderContext>
{
    public Task<Result<IConcreteTypeBuilder>> Process(PipelineContext<IConcreteTypeBuilder, BuilderContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        context.Response.AddGenericTypeArguments(context.Request.SourceModel.GenericTypeArguments);
        context.Response.AddGenericTypeArgumentConstraints(context.Request.SourceModel.GenericTypeArgumentConstraints);

        return Task.FromResult(Result.Continue<IConcreteTypeBuilder>());
    }
}

namespace ClassFramework.Pipelines.Builder.Components;

public class GenericsComponentBuilder : IBuilderComponentBuilder
{
    public IPipelineComponent<BuilderContext, IConcreteTypeBuilder> Build()
        => new GenericsComponent();
}

public class GenericsComponent : IPipelineComponent<BuilderContext, IConcreteTypeBuilder>
{
    public Task<Result> Process(PipelineContext<BuilderContext, IConcreteTypeBuilder> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        context.Response.AddGenericTypeArguments(context.Request.SourceModel.GenericTypeArguments);
        context.Response.AddGenericTypeArgumentConstraints(context.Request.SourceModel.GenericTypeArgumentConstraints);

        return Task.FromResult(Result.Continue());
    }
}

namespace ClassFramework.Pipelines.Builder.Components;

public class GenericsComponentBuilder : IBuilderComponentBuilder
{
    public IPipelineComponent<BuilderContext> Build()
        => new GenericsComponent();
}

public class GenericsComponent : IPipelineComponent<BuilderContext>
{
    public Task<Result> Process(PipelineContext<BuilderContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        context.Request.Builder.AddGenericTypeArguments(context.Request.SourceModel.GenericTypeArguments);
        context.Request.Builder.AddGenericTypeArgumentConstraints(context.Request.SourceModel.GenericTypeArgumentConstraints);

        return Task.FromResult(Result.Continue());
    }
}

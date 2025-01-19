namespace ClassFramework.Pipelines.Builder.Components;

public class PartialComponentBuilder : IBuilderComponentBuilder
{
    public IPipelineComponent<BuilderContext> Build() => new PartialComponent();
}

public class PartialComponent : IPipelineComponent<BuilderContext>
{
    public Task<Result> ProcessAsync(PipelineContext<BuilderContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        context.Request.Builder.WithPartial(context.Request.Settings.CreateAsPartial);

        return Task.FromResult(Result.Success());
    }
}

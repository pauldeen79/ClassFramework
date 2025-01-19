namespace ClassFramework.Pipelines.Builder.Components;

public class GenericsComponent : IPipelineComponent<BuilderContext>
{
    public Task<Result> ProcessAsync(PipelineContext<BuilderContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        context.Request.Builder.AddGenericTypeArguments(context.Request.SourceModel.GenericTypeArguments);
        context.Request.Builder.AddGenericTypeArgumentConstraints(context.Request.SourceModel.GenericTypeArgumentConstraints);

        return Task.FromResult(Result.Success());
    }
}

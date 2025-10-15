namespace ClassFramework.Pipelines.Builder.Components;

public class GenericsComponent : IPipelineComponent<BuilderContext>, IOrderContainer
{
    public int Order => PipelineStage.Process;

    public Task<Result> ProcessAsync(PipelineContext<BuilderContext> context, CancellationToken token)
        => Task.Run(() =>
        {
            context = context.IsNotNull(nameof(context));

            context.Request.Builder.AddGenericTypeArguments(context.Request.SourceModel.GenericTypeArguments);
            context.Request.Builder.AddGenericTypeArgumentConstraints(context.Request.SourceModel.GenericTypeArgumentConstraints);

            return Result.Success();
        }, token);
}

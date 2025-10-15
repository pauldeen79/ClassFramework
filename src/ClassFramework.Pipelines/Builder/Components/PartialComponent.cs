namespace ClassFramework.Pipelines.Builder.Components;

public class PartialComponent : IPipelineComponent<BuilderContext>, IOrderContainer
{
    public int Order => PipelineStage.Process;

    public Task<Result> ProcessAsync(PipelineContext<BuilderContext> context, CancellationToken token)
        => Task.Run(() =>
        {
            context = context.IsNotNull(nameof(context));

            context.Request.Builder.WithPartial(context.Request.Settings.CreateAsPartial);

            return Result.Success();
        }, token);
}

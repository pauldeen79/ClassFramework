namespace ClassFramework.Pipelines.BuilderExtension.Components;

public class PartialComponent : IPipelineComponent<BuilderExtensionContext>, IOrderContainer
{
    public int Order => PipelineStage.Process;

    public Task<Result> ProcessAsync(PipelineContext<BuilderExtensionContext> context, CancellationToken token)
        => Task.Run(() =>
        {
            context = context.IsNotNull(nameof(context));

            context.Request.Builder.WithPartial(context.Request.Settings.CreateAsPartial);

            return Result.Success();
        }, token);
}

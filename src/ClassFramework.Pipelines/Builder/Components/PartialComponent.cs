namespace ClassFramework.Pipelines.Builder.Components;

public class PartialComponent : IPipelineComponent<BuilderContext>, IOrderContainer
{
    public int Order => PipelineStage.Process;

    public Task<Result> ExecuteAsync(BuilderContext context, ICommandService commandService, CancellationToken token)
        => Task.Run(() =>
        {
            context = context.IsNotNull(nameof(context));

            context.Builder.WithPartial(context.Settings.CreateAsPartial);

            return Result.Success();
        }, token);
}

namespace ClassFramework.Pipelines.BuilderExtension.Components;

public class PartialComponent : IPipelineComponent<BuilderExtensionContext>, IOrderContainer
{
    public int Order => PipelineStage.Process;

    public Task<Result> ExecuteAsync(BuilderExtensionContext context, ICommandService commandService, CancellationToken token)
        => Task.Run(() =>
        {
            context = context.IsNotNull(nameof(context));

            context.Builder.WithPartial(context.Settings.CreateAsPartial);

            return Result.Success();
        }, token);
}

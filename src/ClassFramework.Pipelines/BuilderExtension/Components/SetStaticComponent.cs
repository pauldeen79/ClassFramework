namespace ClassFramework.Pipelines.BuilderExtension.Components;

public class SetStaticComponent : IPipelineComponent<BuilderExtensionContext>, IOrderContainer
{
    public int Order => PipelineStage.Process;

    public Task<Result> ExecuteAsync(BuilderExtensionContext context, ICommandService commandService, CancellationToken token)
        => Task.Run(() =>
        {
            context = context.IsNotNull(nameof(context));

            context.Builder.WithStatic();

            return Result.Success();
        }, token);
}

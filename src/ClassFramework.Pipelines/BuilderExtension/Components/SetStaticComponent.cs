namespace ClassFramework.Pipelines.BuilderExtension.Components;

public class SetStaticComponent : IPipelineComponent<BuilderExtensionContext>
{
    public Task<Result> ExecuteAsync(BuilderExtensionContext context, ICommandService commandService, CancellationToken token)
        => Task.Run(() =>
        {
            context = context.IsNotNull(nameof(context));

            context.Builder.WithStatic();

            return Result.Success();
        }, token);
}

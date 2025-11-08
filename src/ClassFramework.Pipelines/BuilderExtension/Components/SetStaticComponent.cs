namespace ClassFramework.Pipelines.BuilderExtension.Components;

public class SetStaticComponent : IPipelineComponent<BuilderExtensionContext, ClassBuilder>
{
    public Task<Result> ExecuteAsync(BuilderExtensionContext context, ClassBuilder response, ICommandService commandService, CancellationToken token)
        => Task.Run(() =>
        {
            context = context.IsNotNull(nameof(context));

            response.WithStatic();

            return Result.Success();
        }, token);
}

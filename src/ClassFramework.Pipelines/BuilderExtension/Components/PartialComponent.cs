namespace ClassFramework.Pipelines.BuilderExtension.Components;

public class PartialComponent : IPipelineComponent<BuilderExtensionContext, ClassBuilder>
{
    public Task<Result> ExecuteAsync(BuilderExtensionContext context, ClassBuilder response, ICommandService commandService, CancellationToken token)
        => Task.Run(() =>
        {
            context = context.IsNotNull(nameof(context));
            response = response.IsNotNull(nameof(response));

            response.WithPartial(context.Settings.CreateAsPartial);

            return Result.Success();
        }, token);
}

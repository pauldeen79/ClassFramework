namespace ClassFramework.Pipelines.Entity.Components;

public class PartialComponent : IPipelineComponent<EntityContext>
{
    public Task<Result> ExecuteAsync(EntityContext context, ICommandService commandService, CancellationToken token)
        => Task.Run(() =>
        {
            context = context.IsNotNull(nameof(context));

            context.Builder.WithPartial(context.Settings.CreateAsPartial);

            return Result.Success();
        }, token);
}

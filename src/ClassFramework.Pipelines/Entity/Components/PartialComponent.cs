namespace ClassFramework.Pipelines.Entity.Components;

public class PartialComponent : IPipelineComponent<EntityContext, ClassBuilder>
{
    public Task<Result> ExecuteAsync(EntityContext context, ClassBuilder response, ICommandService commandService, CancellationToken token)
        => Task.Run(() =>
        {
            context = context.IsNotNull(nameof(context));

            response.WithPartial(context.Settings.CreateAsPartial);

            return Result.Success();
        }, token);
}

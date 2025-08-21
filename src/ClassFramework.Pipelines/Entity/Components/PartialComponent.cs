namespace ClassFramework.Pipelines.Entity.Components;

public class PartialComponent : IPipelineComponent<EntityContext>
{
    public Task<Result> ProcessAsync(PipelineContext<EntityContext> context, CancellationToken token)
        => Task.Run(() =>
        {
            context = context.IsNotNull(nameof(context));

            context.Request.Builder.WithPartial(context.Request.Settings.CreateAsPartial);

            return Result.Success();
        }, token);
}

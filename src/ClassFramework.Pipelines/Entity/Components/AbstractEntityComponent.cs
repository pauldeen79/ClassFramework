namespace ClassFramework.Pipelines.Entity.Components;

public class AbstractEntityComponent : IPipelineComponent<EntityContext>, IOrderContainer
{
    public int Order => PipelineStage.Process;

    public Task<Result> ProcessAsync(PipelineContext<EntityContext> context, CancellationToken token)
        => Task.Run(() =>
        {
            context = context.IsNotNull(nameof(context));

            context.Request.Builder.WithAbstract(context.Request.IsAbstract);

            return Result.Success();
        }, token);
}

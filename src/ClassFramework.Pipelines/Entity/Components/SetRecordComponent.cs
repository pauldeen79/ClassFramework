namespace ClassFramework.Pipelines.Entity.Components;

public class SetRecordComponent : IPipelineComponent<EntityContext>, IOrderContainer
{
    public int Order => PipelineStage.Process;

    public Task<Result> ProcessAsync(PipelineContext<EntityContext> context, CancellationToken token)
        => Task.Run(() =>
        {
            context = context.IsNotNull(nameof(context));

            context.Request.Builder.WithRecord(context.Request.Settings.CreateRecord);

            return Result.Success();
        }, token);
}

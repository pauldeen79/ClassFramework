namespace ClassFramework.Pipelines.Entity.Components;

public class SetRecordComponent : IPipelineComponent<EntityContext>, IOrderContainer
{
    public int Order => PipelineStage.Process;

    public Task<Result> ExecuteAsync(EntityContext context, ICommandService commandService, CancellationToken token)
        => Task.Run(() =>
        {
            context = context.IsNotNull(nameof(context));

            context.Builder.WithRecord(context.Settings.CreateRecord);

            return Result.Success();
        }, token);
}

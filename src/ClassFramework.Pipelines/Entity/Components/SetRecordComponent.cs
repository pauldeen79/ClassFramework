namespace ClassFramework.Pipelines.Entity.Components;

public class SetRecordComponent : IPipelineComponent<EntityContext, ClassBuilder>
{
    public Task<Result> ExecuteAsync(EntityContext context, ClassBuilder response, ICommandService commandService, CancellationToken token)
        => Task.Run(() =>
        {
            context = context.IsNotNull(nameof(context));
            response = response.IsNotNull(nameof(response));

            response.WithRecord(context.Settings.CreateRecord);

            return Result.Success();
        }, token);
}

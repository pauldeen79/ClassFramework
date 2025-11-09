namespace ClassFramework.Pipelines.Entity.Components;

public class SetRecordComponent : IPipelineComponent<GenerateEntityCommand, ClassBuilder>
{
    public Task<Result> ExecuteAsync(GenerateEntityCommand context, ClassBuilder response, ICommandService commandService, CancellationToken token)
        => Task.Run(() =>
        {
            context = context.IsNotNull(nameof(context));
            response = response.IsNotNull(nameof(response));

            response.WithRecord(context.Settings.CreateRecord);

            return Result.Success();
        }, token);
}

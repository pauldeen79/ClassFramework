namespace ClassFramework.Pipelines.Entity.Components;

public class SetRecordComponent : IPipelineComponent<GenerateEntityCommand, ClassBuilder>
{
    public Task<Result> ExecuteAsync(GenerateEntityCommand command, ClassBuilder response, ICommandService commandService, CancellationToken token)
        => Task.Run(() =>
        {
            command = command.IsNotNull(nameof(command));
            response = response.IsNotNull(nameof(response));

            response.WithRecord(command.Settings.CreateRecord);

            return Result.Success();
        }, token);
}

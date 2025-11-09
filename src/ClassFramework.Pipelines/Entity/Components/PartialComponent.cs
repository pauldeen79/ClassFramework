namespace ClassFramework.Pipelines.Entity.Components;

public class PartialComponent : IPipelineComponent<GenerateEntityCommand, ClassBuilder>
{
    public Task<Result> ExecuteAsync(GenerateEntityCommand command, ClassBuilder response, ICommandService commandService, CancellationToken token)
        => Task.Run(() =>
        {
            command = command.IsNotNull(nameof(command));
            response = response.IsNotNull(nameof(response));

            response.WithPartial(command.Settings.CreateAsPartial);

            return Result.Success();
        }, token);
}

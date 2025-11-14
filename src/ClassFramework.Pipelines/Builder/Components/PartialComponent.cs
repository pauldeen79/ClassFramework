namespace ClassFramework.Pipelines.Builder.Components;

public class PartialComponent : IPipelineComponent<GenerateBuilderCommand, ClassBuilder>
{
    public Task<Result> ExecuteAsync(GenerateBuilderCommand command, ClassBuilder response, ICommandService commandService, CancellationToken token)
        => Task.Run(() =>
        {
            command = command.IsNotNull(nameof(command));
            response = response.IsNotNull(nameof(response));

            response.WithPartial(command.Settings.CreateAsPartial);

            return Result.Success();
        }, token);
}

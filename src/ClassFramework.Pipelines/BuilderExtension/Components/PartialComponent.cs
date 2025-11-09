namespace ClassFramework.Pipelines.BuilderExtension.Components;

public class PartialComponent : IPipelineComponent<GenerateBuilderExtensionCommand, ClassBuilder>
{
    public Task<Result> ExecuteAsync(GenerateBuilderExtensionCommand command, ClassBuilder response, ICommandService commandService, CancellationToken token)
        => Task.Run(() =>
        {
            command = command.IsNotNull(nameof(command));
            response = response.IsNotNull(nameof(response));

            response.WithPartial(command.Settings.CreateAsPartial);

            return Result.Success();
        }, token);
}

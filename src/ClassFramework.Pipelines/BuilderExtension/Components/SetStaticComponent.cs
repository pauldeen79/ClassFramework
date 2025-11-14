namespace ClassFramework.Pipelines.BuilderExtension.Components;

public class SetStaticComponent : IPipelineComponent<GenerateBuilderExtensionCommand, ClassBuilder>
{
    public Task<Result> ExecuteAsync(GenerateBuilderExtensionCommand command, ClassBuilder response, ICommandService commandService, CancellationToken token)
        => Task.Run(() =>
        {
            command = command.IsNotNull(nameof(command));
            response = response.IsNotNull(nameof(response));

            response.WithStatic();

            return Result.Success();
        }, token);
}

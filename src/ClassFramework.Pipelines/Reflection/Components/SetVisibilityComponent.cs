namespace ClassFramework.Pipelines.Reflection.Components;

public class SetVisibilityComponent : IPipelineComponent<GenerateTypeFromReflectionCommand, TypeBaseBuilder>
{
    public Task<Result> ExecuteAsync(GenerateTypeFromReflectionCommand command, TypeBaseBuilder response, ICommandService commandService, CancellationToken token)
        => Task.Run(() =>
        {
            command = command.IsNotNull(nameof(command));
            response = response.IsNotNull(nameof(response));

            response.WithVisibility(GetVisibility(command));

            return Result.Success();
        }, token);

    private static Visibility GetVisibility(GenerateTypeFromReflectionCommand command)
    {
        if (command.SourceModel.IsPublic)
        {
            return Visibility.Public;
        }

        return command.SourceModel.IsNotPublic
            ? Visibility.Internal
            : Visibility.Private;
    }
}

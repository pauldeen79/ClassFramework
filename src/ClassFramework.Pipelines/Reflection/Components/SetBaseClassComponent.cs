namespace ClassFramework.Pipelines.Reflection.Components;

public class SetBaseClassComponent : IPipelineComponent<GenerateTypeFromReflectionCommand, TypeBaseBuilder>
{
    public Task<Result> ExecuteAsync(GenerateTypeFromReflectionCommand command, TypeBaseBuilder response, ICommandService commandService, CancellationToken token)
        => Task.Run(() =>
        {
            command = command.IsNotNull(nameof(command));
            response = response.IsNotNull(nameof(response));

            if (response is IBaseClassContainerBuilder baseClassContainerBuilder)
            {
                baseClassContainerBuilder.WithBaseClass(command.SourceModel.GetEntityBaseClass(command.Settings));
            }

            return Result.Success();
        }, token);
}

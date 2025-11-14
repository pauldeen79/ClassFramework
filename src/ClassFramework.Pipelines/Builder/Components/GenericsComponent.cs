namespace ClassFramework.Pipelines.Builder.Components;

public class GenericsComponent : IPipelineComponent<GenerateBuilderCommand, ClassBuilder>
{
    public Task<Result> ExecuteAsync(GenerateBuilderCommand command, ClassBuilder response, ICommandService commandService, CancellationToken token)
        => Task.Run(() =>
        {
            command = command.IsNotNull(nameof(command));
            response = response.IsNotNull(nameof(response));

            response.AddGenericTypeArguments(command.SourceModel.GenericTypeArguments);
            response.AddGenericTypeArgumentConstraints(command.SourceModel.GenericTypeArgumentConstraints);

            return Result.Success();
        }, token);
}

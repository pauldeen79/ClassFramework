namespace ClassFramework.Pipelines.Entity.Components;

public class AddGenericsComponent : IPipelineComponent<GenerateEntityCommand, ClassBuilder>
{
    public Task<Result> ExecuteAsync(GenerateEntityCommand command, ClassBuilder response, ICommandService commandService, CancellationToken token)
        => Task.Run(() =>
        {
            command = command.IsNotNull(nameof(command));
            response = response.IsNotNull(nameof(response));

            response
                .AddGenericTypeArguments(command.SourceModel.GenericTypeArguments)
                .AddGenericTypeArgumentConstraints(command.SourceModel.GenericTypeArgumentConstraints);

            return Result.Success();
        }, token);
}

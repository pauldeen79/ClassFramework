namespace ClassFramework.Pipelines.Interface.Components;

public class GenericsComponent : IPipelineComponent<GenerateInterfaceCommand, InterfaceBuilder>
{
    public Task<Result> ExecuteAsync(GenerateInterfaceCommand command, InterfaceBuilder response, ICommandService commandService, CancellationToken token)
        => Task.Run(() =>
        {
            command = command.IsNotNull(nameof(command));
            response = response.IsNotNull(nameof(response));

            response.AddGenericTypeArguments(command.SourceModel.GenericTypeArguments);
            response.AddGenericTypeArgumentConstraints(command.SourceModel.GenericTypeArgumentConstraints);

            return Result.Success();
        }, token);
}

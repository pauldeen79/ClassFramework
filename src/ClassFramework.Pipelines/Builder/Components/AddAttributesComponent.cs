namespace ClassFramework.Pipelines.Builder.Components;

public class AddAttributesComponent : IPipelineComponent<GenerateBuilderCommand, ClassBuilder>
{
    public Task<Result> ExecuteAsync(GenerateBuilderCommand command, ClassBuilder response, ICommandService commandService, CancellationToken token)
        => Task.Run(() =>
        {
            command = command.IsNotNull(nameof(command));
            response = response.IsNotNull(nameof(response));

            response.AddAttributes(command.GetAtributes(command.SourceModel.Attributes));

            return Result.Success();
        }, token);
}

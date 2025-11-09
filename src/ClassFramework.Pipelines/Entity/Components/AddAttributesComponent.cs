namespace ClassFramework.Pipelines.Entity.Components;

public class AddAttributesComponent : IPipelineComponent<GenerateEntityCommand, ClassBuilder>
{
    public Task<Result> ExecuteAsync(GenerateEntityCommand command, ClassBuilder response, ICommandService commandService, CancellationToken token)
        => Task.Run(() =>
        {
            command = command.IsNotNull(nameof(command));
            response = response.IsNotNull(nameof(response));

            response.AddAttributes(command.GetAtributes(command.SourceModel.Attributes));

            return Result.Success();
        }, token);
}

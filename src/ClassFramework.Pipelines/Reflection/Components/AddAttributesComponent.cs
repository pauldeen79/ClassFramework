namespace ClassFramework.Pipelines.Reflection.Components;

public class AddAttributesComponent : IPipelineComponent<GenerateTypeFromReflectionCommand, TypeBaseBuilder>
{
    public Task<Result> ExecuteAsync(GenerateTypeFromReflectionCommand command, TypeBaseBuilder response, ICommandService commandService, CancellationToken token)
        => Task.Run(() =>
        {
            command = command.IsNotNull(nameof(command));
            response = response.IsNotNull(nameof(response));

            if (!command.Settings.CopyAttributes)
            {
                return Result.Continue();
            }

            response.AddAttributes(command.SourceModel.GetCustomAttributes(true).ToAttributes(
                x => command.MapAttribute(x.ConvertToDomainAttribute(command.InitializeDelegate)),
                command.Settings.CopyAttributes,
                command.Settings.CopyAttributePredicate));

            return Result.Success();
        }, token);
}

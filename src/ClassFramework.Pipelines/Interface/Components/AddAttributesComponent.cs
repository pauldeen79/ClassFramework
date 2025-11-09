namespace ClassFramework.Pipelines.Interface.Components;

public class AddAttributesComponent : IPipelineComponent<GenerateInterfaceCommand, InterfaceBuilder>
{
    public Task<Result> ExecuteAsync(GenerateInterfaceCommand command, InterfaceBuilder response, ICommandService commandService, CancellationToken token)
        => Task.Run(() =>
        {
            command = command.IsNotNull(nameof(command));
            response = response.IsNotNull(nameof(response));

            if (!command.Settings.CopyAttributes)
            {
                return Result.Continue();
            }

            response.AddAttributes(command.SourceModel.Attributes
                .Where(x => command.Settings.CopyAttributePredicate?.Invoke(x) ?? true)
                .Select(x => command.MapAttribute(x).ToBuilder()));

            return Result.Success();
        }, token);
}

namespace ClassFramework.Pipelines.Reflection.Components;

public class AddInterfacesComponent : IPipelineComponent<GenerateTypeFromReflectionCommand, TypeBaseBuilder>
{
    public Task<Result> ExecuteAsync(GenerateTypeFromReflectionCommand command, TypeBaseBuilder response, ICommandService commandService, CancellationToken token)
        => Task.Run(() =>
        {
            command = command.IsNotNull(nameof(command));
            response = response.IsNotNull(nameof(response));

            if (!command.Settings.CopyInterfaces)
            {
                return Result.Continue();
            }

            response.AddInterfaces(
                command.SourceModel.GetInterfaces()
                    .Where(x => !(command.SourceModel.IsRecord() && x.FullName.StartsWith($"System.IEquatable`1[[{command.SourceModel.FullName}")))
                    .Select(x => command.GetMappedTypeName(x, command.SourceModel))
                    .Where(x => command.Settings.CopyInterfacePredicate?.Invoke(x) ?? true)
                    .Select(x => command.MapTypeName(x))
            );

            return Result.Success();
        }, token);
}

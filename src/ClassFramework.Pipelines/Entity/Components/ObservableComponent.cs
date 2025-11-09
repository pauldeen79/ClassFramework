namespace ClassFramework.Pipelines.Entity.Components;

public class ObservableComponent : IPipelineComponent<GenerateEntityCommand, ClassBuilder>
{
    public Task<Result> ExecuteAsync(GenerateEntityCommand command, ClassBuilder response, ICommandService commandService, CancellationToken token)
        => Task.Run(() =>
        {
            command = command.IsNotNull(nameof(command));
            response = response.IsNotNull(nameof(response));

            if (!command.Settings.CreateAsObservable
                && !command.SourceModel.Interfaces.Any(x => x == typeof(INotifyPropertyChanged).FullName))
            {
                return Result.Continue();
            }

            if (command.Settings.EnableInheritance
                && command.Settings.BaseClass is not null)
            {
                // Already present in base class
                return Result.Continue();
            }

            if (!command.SourceModel.Interfaces.Any(x => x == typeof(INotifyPropertyChanged).FullName))
            {
                // Only add the interface when it's not present yet :)
                response.AddInterfaces(typeof(INotifyPropertyChanged));
            }

            response.AddObservableMembers();

            return Result.Success();
        }, token);
}

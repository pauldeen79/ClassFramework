namespace ClassFramework.Pipelines.Entity.Components;

public class ObservableComponent : IPipelineComponent<EntityContext, ClassBuilder>
{
    public Task<Result> ExecuteAsync(EntityContext context, ClassBuilder response, ICommandService commandService, CancellationToken token)
        => Task.Run(() =>
        {
            context = context.IsNotNull(nameof(context));
            response = response.IsNotNull(nameof(response));

            if (!context.Settings.CreateAsObservable
                && !context.SourceModel.Interfaces.Any(x => x == typeof(INotifyPropertyChanged).FullName))
            {
                return Result.Continue();
            }

            if (context.Settings.EnableInheritance
                && context.Settings.BaseClass is not null)
            {
                // Already present in base class
                return Result.Continue();
            }

            if (!context.SourceModel.Interfaces.Any(x => x == typeof(INotifyPropertyChanged).FullName))
            {
                // Only add the interface when it's not present yet :)
                response.AddInterfaces(typeof(INotifyPropertyChanged));
            }

            response.AddObservableMembers();

            return Result.Success();
        }, token);
}

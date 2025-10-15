namespace ClassFramework.Pipelines.Entity.Components;

public class ObservableComponent : IPipelineComponent<EntityContext>, IOrderContainer
{
    public int Order => PipelineStage.Process;

    public Task<Result> ProcessAsync(PipelineContext<EntityContext> context, CancellationToken token)
        => Task.Run(() =>
        {
            context = context.IsNotNull(nameof(context));

            if (!context.Request.Settings.CreateAsObservable
                && !context.Request.SourceModel.Interfaces.Any(x => x == typeof(INotifyPropertyChanged).FullName))
            {
                return Result.Continue();
            }

            if (context.Request.Settings.EnableInheritance
                && context.Request.Settings.BaseClass is not null)
            {
                // Already present in base class
                return Result.Continue();
            }

            if (!context.Request.SourceModel.Interfaces.Any(x => x == typeof(INotifyPropertyChanged).FullName))
            {
                // Only add the interface when it's not present yet :)
                context.Request.Builder.AddInterfaces(typeof(INotifyPropertyChanged));
            }

            context.Request.Builder.AddObservableMembers();

            return Result.Success();
        }, token);
}

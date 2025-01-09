namespace ClassFramework.Pipelines.Entity.Components;

public class ObservableComponentBuilder : IEntityComponentBuilder
{
    public IPipelineComponent<EntityContext> Build()
        => new ObservableComponent();
}

public class ObservableComponent : IPipelineComponent<EntityContext>
{
    public Task<Result> Process(PipelineContext<EntityContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        if (!context.Request.Settings.CreateAsObservable
            && !context.Request.SourceModel.Interfaces.Any(x => x == typeof(INotifyPropertyChanged).FullName))
        {
            return Task.FromResult(Result.Success());
        }

        if (context.Request.Settings.EnableInheritance
            && context.Request.Settings.BaseClass is not null)
        {
            // Already present in base class
            return Task.FromResult(Result.Success());
        }

        if (!context.Request.SourceModel.Interfaces.Any(x => x == typeof(INotifyPropertyChanged).FullName))
        {
            // Only add the interface when it's not present yet :)
            context.Request.Builder.AddInterfaces(typeof(INotifyPropertyChanged));
        }

        context.Request.Builder.AddObservableMembers();

        return Task.FromResult(Result.Success());
    }
}

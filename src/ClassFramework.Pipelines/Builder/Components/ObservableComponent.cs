namespace ClassFramework.Pipelines.Builder.Components;

public class ObservableComponentBuilder : IBuilderComponentBuilder
{
    public IPipelineComponent<BuilderContext> Build()
        => new ObservableComponent();
}

public class ObservableComponent : IPipelineComponent<BuilderContext>
{
    public Task<Result> ProcessAsync(PipelineContext<BuilderContext> context, CancellationToken token)
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

        if (context.Request.IsBuilderForAbstractEntity && context.Request.IsAbstractBuilder)
        {
            // Already present in non-generic base class
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

namespace ClassFramework.Pipelines.Builder.Features;

public class ObservableComponentBuilder : IBuilderComponentBuilder
{
    public IPipelineComponent<BuilderContext, IConcreteTypeBuilder> Build()
        => new ObservableComponent();
}

public class ObservableComponent : IPipelineComponent<BuilderContext, IConcreteTypeBuilder>
{
    public Task<Result> Process(PipelineContext<BuilderContext, IConcreteTypeBuilder> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        if (!context.Request.Settings.CreateAsObservable
            && !context.Request.SourceModel.Interfaces.Any(x => x == typeof(INotifyPropertyChanged).FullName))
        {
            return Task.FromResult(Result.Continue());
        }

        if (context.Request.Settings.EnableInheritance
            && context.Request.Settings.BaseClass is not null)
        {
            // Already present in base class
            return Task.FromResult(Result.Continue());
        }

        if (context.Request.IsBuilderForAbstractEntity && context.Request.IsAbstractBuilder)
        {
            // Already present in non-generic base class
            return Task.FromResult(Result.Continue());
        }

        if (!context.Request.SourceModel.Interfaces.Any(x => x == typeof(INotifyPropertyChanged).FullName))
        {
            // Only add the interface when it's not present yet :)
            context.Response.AddInterfaces(typeof(INotifyPropertyChanged));
        }

        context.Response.AddObservableMembers();

        return Task.FromResult(Result.Continue());
    }
}

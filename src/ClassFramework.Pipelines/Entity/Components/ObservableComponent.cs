namespace ClassFramework.Pipelines.Entity.Components;

public class ObservableComponentBuilder : IEntityComponentBuilder
{
    public IPipelineComponent<EntityContext, IConcreteTypeBuilder> Build()
        => new ObservableComponent();
}

public class ObservableComponent : IPipelineComponent<EntityContext, IConcreteTypeBuilder>
{
    public Task<Result> Process(PipelineContext<EntityContext, IConcreteTypeBuilder> context, CancellationToken token)
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

        if (!context.Request.SourceModel.Interfaces.Any(x => x == typeof(INotifyPropertyChanged).FullName))
        {
            // Only add the interface when it's not present yet :)
            context.Response.AddInterfaces(typeof(INotifyPropertyChanged));
        }

        context.Response.AddObservableMembers();

        return Task.FromResult(Result.Continue());
    }
}

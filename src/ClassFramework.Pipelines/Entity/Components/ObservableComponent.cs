namespace ClassFramework.Pipelines.Entity.Features;

public class ObservableComponentBuilder : IEntityComponentBuilder
{
    public IPipelineComponent<IConcreteTypeBuilder, EntityContext> Build()
        => new ObservableComponent();
}

public class ObservableComponent : IPipelineComponent<IConcreteTypeBuilder, EntityContext>
{
    public Task<Result<IConcreteTypeBuilder>> Process(PipelineContext<IConcreteTypeBuilder, EntityContext> context)
    {
        context = context.IsNotNull(nameof(context));

        if (!context.Context.Settings.CreateAsObservable
            && !context.Context.SourceModel.Interfaces.Any(x => x == typeof(INotifyPropertyChanged).FullName))
        {
            return Task.FromResult(Result.Continue<IConcreteTypeBuilder>());
        }

        if (context.Context.Settings.EnableInheritance
            && context.Context.Settings.BaseClass is not null)
        {
            // Already present in base class
            return Task.FromResult(Result.Continue<IConcreteTypeBuilder>());
        }

        if (!context.Context.SourceModel.Interfaces.Any(x => x == typeof(INotifyPropertyChanged).FullName))
        {
            // Only add the interface when it's not present yet :)
            context.Model.AddInterfaces(typeof(INotifyPropertyChanged));
        }

        context.Model.AddObservableMembers();

        return Task.FromResult(Result.Continue<IConcreteTypeBuilder>());
    }
}

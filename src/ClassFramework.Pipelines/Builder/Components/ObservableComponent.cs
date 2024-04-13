namespace ClassFramework.Pipelines.Builder.Features;

public class ObservableComponentBuilder : IBuilderComponentBuilder
{
    public IPipelineComponent<IConcreteTypeBuilder, BuilderContext> Build()
        => new ObservableComponent();
}

public class ObservableComponent : IPipelineComponent<IConcreteTypeBuilder, BuilderContext>
{
    public Task<Result<IConcreteTypeBuilder>> Process(PipelineContext<IConcreteTypeBuilder, BuilderContext> context)
    {
        context = context.IsNotNull(nameof(context));

        if (!context.Context.Settings.CreateAsObservable
            && !context.Context.SourceModel.Interfaces.Any(x => x == typeof(INotifyPropertyChanged).FullName))
        {
            return Result.Continue<IConcreteTypeBuilder>();
        }

        if (context.Context.Settings.EnableInheritance
            && context.Context.Settings.BaseClass is not null)
        {
            // Already present in base class
            return Result.Continue<IConcreteTypeBuilder>();
        }

        if (context.Context.IsBuilderForAbstractEntity && context.Context.IsAbstractBuilder)
        {
            // Already present in non-generic base class
            return Result.Continue<IConcreteTypeBuilder>();
        }

        if (!context.Context.SourceModel.Interfaces.Any(x => x == typeof(INotifyPropertyChanged).FullName))
        {
            // Only add the interface when it's not present yet :)
            context.Model.AddInterfaces(typeof(INotifyPropertyChanged));
        }

        context.Model.AddObservableMembers();

        return Result.Continue<IConcreteTypeBuilder>();
    }
}

namespace ClassFramework.Pipelines.Entity.Features;

public class AddInterfacesComponentBuilder : IEntityComponentBuilder
{
    public IPipelineComponent<IConcreteTypeBuilder, EntityContext> Build()
        => new AddInterfacesComponent();
}

public class AddInterfacesComponent : IPipelineComponent<IConcreteTypeBuilder, EntityContext>
{
    public Task<Result<IConcreteTypeBuilder>> Process(PipelineContext<IConcreteTypeBuilder, EntityContext> context)
    {
        context = context.IsNotNull(nameof(context));

        if (!context.Context.Settings.CopyInterfaces)
        {
            return Result.Continue<IConcreteTypeBuilder>();
        }

        var baseClass = context.Context.SourceModel.GetEntityBaseClass(context.Context.Settings.EnableInheritance, context.Context.Settings.BaseClass);

        context.Model.AddInterfaces(context.Context.SourceModel.Interfaces
            .Where(x => context.Context.Settings.CopyInterfacePredicate?.Invoke(x) ?? true)
            .Where(x => x != baseClass)
            .Select(x => context.Context.MapTypeName(x.FixTypeName()))
            .Where(x => !string.IsNullOrEmpty(x)));

        return Result.Continue<IConcreteTypeBuilder>();
    }
}

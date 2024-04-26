namespace ClassFramework.Pipelines.Entity.Features;

public class AddInterfacesComponentBuilder : IEntityComponentBuilder
{
    public IPipelineComponent<EntityContext, IConcreteTypeBuilder> Build()
        => new AddInterfacesComponent();
}

public class AddInterfacesComponent : IPipelineComponent<EntityContext, IConcreteTypeBuilder>
{
    public Task<Result> Process(PipelineContext<EntityContext, IConcreteTypeBuilder> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        if (!context.Request.Settings.CopyInterfaces)
        {
            return Task.FromResult(Result.Continue());
        }

        var baseClass = context.Request.SourceModel.GetEntityBaseClass(context.Request.Settings.EnableInheritance, context.Request.Settings.BaseClass);

        context.Response.AddInterfaces(context.Request.SourceModel.Interfaces
            .Where(x => context.Request.Settings.CopyInterfacePredicate?.Invoke(x) ?? true)
            .Where(x => x != baseClass)
            .Select(x => context.Request.MapTypeName(x.FixTypeName()))
            .Where(x => !string.IsNullOrEmpty(x)));

        return Task.FromResult(Result.Continue());
    }
}

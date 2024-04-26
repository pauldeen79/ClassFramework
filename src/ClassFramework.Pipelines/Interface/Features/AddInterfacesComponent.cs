namespace ClassFramework.Pipelines.Interface.Features;

public class AddInterfacesComponentBuilder : IInterfaceComponentBuilder
{
    public IPipelineComponent<InterfaceContext, InterfaceBuilder> Build()
        => new AddInterfacesComponent();
}

public class AddInterfacesComponent : IPipelineComponent<InterfaceContext, InterfaceBuilder>
{
    public Task<Result> Process(PipelineContext<InterfaceContext, InterfaceBuilder> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        if (!context.Request.Settings.CopyInterfaces)
        {
            return Task.FromResult(Result.Continue());
        }

        context.Response.AddInterfaces(context.Request.SourceModel.Interfaces
            .Where(x => context.Request.Settings.CopyInterfacePredicate?.Invoke(x) ?? true)
            .Select(x => context.Request.MapTypeName(x.FixTypeName()))
            .Where(x => !string.IsNullOrEmpty(x)));

        return Task.FromResult(Result.Continue());
    }
}

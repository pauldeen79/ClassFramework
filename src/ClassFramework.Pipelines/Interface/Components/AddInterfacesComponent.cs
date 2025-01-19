namespace ClassFramework.Pipelines.Interface.Components;

public class AddInterfacesComponentBuilder : IInterfaceComponentBuilder
{
    public IPipelineComponent<InterfaceContext> Build()
        => new AddInterfacesComponent();
}

public class AddInterfacesComponent : IPipelineComponent<InterfaceContext>
{
    public Task<Result> ProcessAsync(PipelineContext<InterfaceContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        if (!context.Request.Settings.CopyInterfaces)
        {
            return Task.FromResult(Result.Success());
        }

        context.Request.Builder.AddInterfaces(context.Request.SourceModel.Interfaces
            .Where(x => context.Request.Settings.CopyInterfacePredicate?.Invoke(x) ?? true)
            .Select(x => context.Request.MapTypeName(x.FixTypeName()))
            .Where(x => !string.IsNullOrEmpty(x)));

        return Task.FromResult(Result.Success());
    }
}

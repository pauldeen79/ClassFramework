namespace ClassFramework.Pipelines.Interface.Features;

public class AddInterfacesComponentBuilder : IInterfaceComponentBuilder
{
    public IPipelineComponent<InterfaceBuilder, InterfaceContext> Build()
        => new AddInterfacesComponent();
}

public class AddInterfacesComponent : IPipelineComponent<InterfaceBuilder, InterfaceContext>
{
    public Task<Result<InterfaceBuilder>> Process(PipelineContext<InterfaceBuilder, InterfaceContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        if (!context.Context.Settings.CopyInterfaces)
        {
            return Task.FromResult(Result.Continue<InterfaceBuilder>());
        }

        context.Model.AddInterfaces(context.Context.SourceModel.Interfaces
            .Where(x => context.Context.Settings.CopyInterfacePredicate?.Invoke(x) ?? true)
            .Select(x => context.Context.MapTypeName(x.FixTypeName()))
            .Where(x => !string.IsNullOrEmpty(x)));

        return Task.FromResult(Result.Continue<InterfaceBuilder>());
    }
}

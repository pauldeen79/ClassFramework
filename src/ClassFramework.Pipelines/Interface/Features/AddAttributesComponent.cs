namespace ClassFramework.Pipelines.Interface.Features;

public class AddAttributesComponentBuilder : IInterfaceComponentBuilder
{
    public IPipelineComponent<InterfaceBuilder, InterfaceContext> Build()
        => new AddAttributesComponent();
}

public class AddAttributesComponent : IPipelineComponent<InterfaceBuilder, InterfaceContext>
{
    public Task<Result<InterfaceBuilder>> Process(PipelineContext<InterfaceBuilder, InterfaceContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        if (!context.Context.Settings.CopyAttributes)
        {
            return Task.FromResult(Result.Continue<InterfaceBuilder>());
        }

        context.Model.AddAttributes(context.Context.SourceModel.Attributes
            .Where(x => context.Context.Settings.CopyAttributePredicate?.Invoke(x) ?? true)
            .Select(x => context.Context.MapAttribute(x).ToBuilder()));

        return Task.FromResult(Result.Continue<InterfaceBuilder>());
    }
}

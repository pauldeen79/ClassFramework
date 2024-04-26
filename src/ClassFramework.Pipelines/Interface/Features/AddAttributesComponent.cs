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

        if (!context.Request.Settings.CopyAttributes)
        {
            return Task.FromResult(Result.Continue<InterfaceBuilder>());
        }

        context.Response.AddAttributes(context.Request.SourceModel.Attributes
            .Where(x => context.Request.Settings.CopyAttributePredicate?.Invoke(x) ?? true)
            .Select(x => context.Request.MapAttribute(x).ToBuilder()));

        return Task.FromResult(Result.Continue<InterfaceBuilder>());
    }
}

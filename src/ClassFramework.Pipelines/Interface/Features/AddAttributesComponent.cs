namespace ClassFramework.Pipelines.Interface.Features;

public class AddAttributesComponentBuilder : IInterfaceComponentBuilder
{
    public IPipelineComponent<InterfaceContext, InterfaceBuilder> Build()
        => new AddAttributesComponent();
}

public class AddAttributesComponent : IPipelineComponent<InterfaceContext, InterfaceBuilder>
{
    public Task<Result> Process(PipelineContext<InterfaceContext, InterfaceBuilder> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        if (!context.Request.Settings.CopyAttributes)
        {
            return Task.FromResult(Result.Continue());
        }

        context.Response.AddAttributes(context.Request.SourceModel.Attributes
            .Where(x => context.Request.Settings.CopyAttributePredicate?.Invoke(x) ?? true)
            .Select(x => context.Request.MapAttribute(x).ToBuilder()));

        return Task.FromResult(Result.Continue());
    }
}

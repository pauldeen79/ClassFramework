namespace ClassFramework.Pipelines.Interface.Components;

public class AddAttributesComponentBuilder : IInterfaceComponentBuilder
{
    public IPipelineComponent<InterfaceContext> Build()
        => new AddAttributesComponent();
}

public class AddAttributesComponent : IPipelineComponent<InterfaceContext>
{
    public Task<Result> ProcessAsync(PipelineContext<InterfaceContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        if (!context.Request.Settings.CopyAttributes)
        {
            return Task.FromResult(Result.Success());
        }

        context.Request.Builder.AddAttributes(context.Request.SourceModel.Attributes
            .Where(x => context.Request.Settings.CopyAttributePredicate?.Invoke(x) ?? true)
            .Select(x => context.Request.MapAttribute(x).ToBuilder()));

        return Task.FromResult(Result.Success());
    }
}

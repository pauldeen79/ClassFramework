namespace ClassFramework.Pipelines.Interface.Components;

public class PartialComponentBuilder : IInterfaceComponentBuilder
{
    public IPipelineComponent<InterfaceContext> Build() => new PartialComponent();
}

public class PartialComponent : IPipelineComponent<InterfaceContext>
{
    public Task<Result> Process(PipelineContext<InterfaceContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        context.Request.Builder.WithPartial(context.Request.Settings.CreateAsPartial);

        return Task.FromResult(Result.Success());
    }
}

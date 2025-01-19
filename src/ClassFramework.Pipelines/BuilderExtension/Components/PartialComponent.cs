namespace ClassFramework.Pipelines.BuilderExtension.Components;

public class PartialComponentBuilder : IBuilderExtensionComponentBuilder
{
    public IPipelineComponent<BuilderExtensionContext> Build() => new PartialComponent();
}

public class PartialComponent : IPipelineComponent<BuilderExtensionContext>
{
    public Task<Result> ProcessAsync(PipelineContext<BuilderExtensionContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        context.Request.Builder.WithPartial(context.Request.Settings.CreateAsPartial);

        return Task.FromResult(Result.Success());
    }
}

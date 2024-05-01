namespace ClassFramework.Pipelines.BuilderExtension.Components;

public class SetStaticComponentBuilder : IBuilderExtensionComponentBuilder
{
    public IPipelineComponent<BuilderExtensionContext> Build()
        => new SetStaticComponent();
}

public class SetStaticComponent : IPipelineComponent<BuilderExtensionContext>
{
    public Task<Result> Process(PipelineContext<BuilderExtensionContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        context.Request.Builder.WithStatic();

        return Task.FromResult(Result.Continue());
    }
}

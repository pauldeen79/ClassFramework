namespace ClassFramework.Pipelines.BuilderExtension.Components;

public class PartialComponentBuilder : IBuilderExtensionComponentBuilder
{
    public IPipelineComponent<BuilderExtensionContext, IConcreteTypeBuilder> Build() => new PartialComponent();
}

public class PartialComponent : IPipelineComponent<BuilderExtensionContext, IConcreteTypeBuilder>
{
    public Task<Result> Process(PipelineContext<BuilderExtensionContext, IConcreteTypeBuilder> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        context.Response.WithPartial(context.Request.Settings.CreateAsPartial);

        return Task.FromResult(Result.Continue());
    }
}

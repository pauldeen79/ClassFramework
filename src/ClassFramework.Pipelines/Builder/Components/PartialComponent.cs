namespace ClassFramework.Pipelines.Builder.Features;

public class PartialComponentBuilder : IBuilderComponentBuilder
{
    public IPipelineComponent<BuilderContext, IConcreteTypeBuilder> Build() => new PartialComponent();
}

public class PartialComponent : IPipelineComponent<BuilderContext, IConcreteTypeBuilder>
{
    public Task<Result> Process(PipelineContext<BuilderContext, IConcreteTypeBuilder> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        context.Response.WithPartial(context.Request.Settings.CreateAsPartial);

        return Task.FromResult(Result.Continue());
    }
}

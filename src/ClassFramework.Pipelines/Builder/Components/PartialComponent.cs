namespace ClassFramework.Pipelines.Builder.Features;

public class PartialComponentBuilder : IBuilderComponentBuilder
{
    public IPipelineComponent<IConcreteTypeBuilder, BuilderContext> Build() => new PartialComponent();
}

public class PartialComponent : IPipelineComponent<IConcreteTypeBuilder, BuilderContext>
{
    public Task<Result<IConcreteTypeBuilder>> Process(PipelineContext<IConcreteTypeBuilder, BuilderContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        context.Model.WithPartial(context.Context.Settings.CreateAsPartial);

        return Task.FromResult(Result.Continue<IConcreteTypeBuilder>());
    }
}

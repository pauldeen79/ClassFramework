namespace ClassFramework.Pipelines.Shared.Features;

public class PartialFeatureBuilder : ISharedFeatureBuilder
{
    public IPipelineFeature<IConcreteTypeBuilder> Build()
        => new PartialFeature();

    public IBuilder<IPipelineFeature<IConcreteTypeBuilder, TContext>> BuildFor<TContext>()
        => new PartialFeatureBuilder<TContext>();
}

public class PartialFeature : IPipelineFeature<IConcreteTypeBuilder>
{
    public Result<IConcreteTypeBuilder> Process(PipelineContext<IConcreteTypeBuilder> context)
    {
        context = context.IsNotNull(nameof(context));

        context.Model.Partial = true;

        return Result.Continue<IConcreteTypeBuilder>();
    }

    public IBuilder<IPipelineFeature<IConcreteTypeBuilder>> ToBuilder()
        => new PartialFeatureBuilder();
}

public class PartialFeatureBuilder<TContext> : IBuilder<IPipelineFeature<IConcreteTypeBuilder, TContext>>
{
    public IPipelineFeature<IConcreteTypeBuilder, TContext> Build()
        => new PipelineFeatureWrapper<IConcreteTypeBuilder, TContext>(() => new PartialFeature());
}

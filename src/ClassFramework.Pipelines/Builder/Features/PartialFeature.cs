namespace ClassFramework.Pipelines.Builder.Features;

public class PartialFeatureBuilder : IBuilderFeatureBuilder
{
    public IPipelineFeature<IConcreteTypeBuilder, BuilderContext> Build() => new PartialFeature();
}

public class PartialFeature : IPipelineFeature<IConcreteTypeBuilder, BuilderContext>
{
    public Result<IConcreteTypeBuilder> Process(PipelineContext<IConcreteTypeBuilder, BuilderContext> context)
    {
        context = context.IsNotNull(nameof(context));

        context.Model.WithPartial(context.Context.Settings.CreateAsPartial);

        return Result.Continue<IConcreteTypeBuilder>();
    }

    public IBuilder<IPipelineFeature<IConcreteTypeBuilder, BuilderContext>> ToBuilder()
        => new PartialFeatureBuilder();
}

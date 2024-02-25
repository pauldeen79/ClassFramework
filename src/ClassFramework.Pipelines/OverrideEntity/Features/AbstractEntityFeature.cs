namespace ClassFramework.Pipelines.OverrideEntity.Features;

public class AbstractEntityFeatureBuilder : IOverrideEntityFeatureBuilder
{
    public IPipelineFeature<IConcreteTypeBuilder, OverrideEntityContext> Build()
        => new AbstractEntityFeature();
}

public class AbstractEntityFeature : IPipelineFeature<IConcreteTypeBuilder, OverrideEntityContext>
{
    public Result<IConcreteTypeBuilder> Process(PipelineContext<IConcreteTypeBuilder, OverrideEntityContext> context)
    {
        context = context.IsNotNull(nameof(context));

        if (context.Model is ClassBuilder cls)
        {
            cls.WithAbstract(context.Context.IsAbstract);
        }

        return Result.Continue<IConcreteTypeBuilder>();
    }

    public IBuilder<IPipelineFeature<IConcreteTypeBuilder, OverrideEntityContext>> ToBuilder()
        => new AbstractEntityFeatureBuilder();
}

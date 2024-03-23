namespace ClassFramework.Pipelines.Reflection.Features;

public class SetBaseClassFeatureBuilder : IReflectionFeatureBuilder
{
    public IPipelineFeature<TypeBaseBuilder, ReflectionContext> Build()
        => new SetBaseClassFeature();
}

public class SetBaseClassFeature : IPipelineFeature<TypeBaseBuilder, ReflectionContext>
{
    public Result<TypeBaseBuilder> Process(PipelineContext<TypeBaseBuilder, ReflectionContext> context)
    {
        context = context.IsNotNull(nameof(context));

        if (context.Model is IBaseClassContainerBuilder baseClassContainerBuilder)
        {
            baseClassContainerBuilder.WithBaseClass(context.Context.SourceModel.GetEntityBaseClass(context.Context.Settings));
        }

        return Result.Continue<TypeBaseBuilder>();
    }

    public IBuilder<IPipelineFeature<TypeBaseBuilder, ReflectionContext>> ToBuilder()
        => new SetBaseClassFeatureBuilder();
}

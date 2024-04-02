namespace ClassFramework.Pipelines.Reflection.Features;

public class AddGenericTypeArgumentsFeatureBuilder : IReflectionFeatureBuilder
{
    public IPipelineFeature<TypeBaseBuilder, ReflectionContext> Build()
        => new AddGenericTypeArgumentsFeature();
}

public class AddGenericTypeArgumentsFeature : IPipelineFeature<TypeBaseBuilder, ReflectionContext>
{
    public Result<TypeBaseBuilder> Process(PipelineContext<TypeBaseBuilder, ReflectionContext> context)
    {
        context = context.IsNotNull(nameof(context));

        context.Model.AddGenericTypeArguments(context.Context.SourceModel.GetGenericTypeArguments());

        return Result.Continue<TypeBaseBuilder>();
    }

    public IBuilder<IPipelineFeature<TypeBaseBuilder, ReflectionContext>> ToBuilder()
        => new AddGenericTypeArgumentsFeatureBuilder();
}

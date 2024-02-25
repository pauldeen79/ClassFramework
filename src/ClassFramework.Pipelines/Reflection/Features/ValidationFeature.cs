namespace ClassFramework.Pipelines.Reflection.Features;

public class ValidationFeatureBuilder : IReflectionFeatureBuilder
{
    public IPipelineFeature<TypeBaseBuilder, ReflectionContext> Build() => new ValidationFeature();
}

public class ValidationFeature : IPipelineFeature<TypeBaseBuilder, ReflectionContext>
{
    public Result<TypeBaseBuilder> Process(PipelineContext<TypeBaseBuilder, ReflectionContext> context)
    {
        context = context.IsNotNull(nameof(context));

        if (!context.Context.Settings.AllowGenerationWithoutProperties
            && context.Context.SourceModel.GetProperties().Length == 0)
        {
            return Result.Invalid<TypeBaseBuilder>("To create a class, there must be at least one property");
        }
        
        return Result.Continue<TypeBaseBuilder>();
    }

    public IBuilder<IPipelineFeature<TypeBaseBuilder, ReflectionContext>> ToBuilder()
        => new ValidationFeatureBuilder();
}

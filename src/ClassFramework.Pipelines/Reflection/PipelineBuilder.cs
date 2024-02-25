namespace ClassFramework.Pipelines.Reflection;

public class PipelineBuilder : PipelineBuilder<TypeBaseBuilder, ReflectionContext>
{
    public PipelineBuilder(IEnumerable<IReflectionFeatureBuilder> reflectionFeatureBuilders)
    {
        AddFeatures(reflectionFeatureBuilders);
    }
}

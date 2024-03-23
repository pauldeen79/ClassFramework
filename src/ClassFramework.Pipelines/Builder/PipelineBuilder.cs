namespace ClassFramework.Pipelines.Builder;

public class PipelineBuilder : PipelineBuilder<IConcreteTypeBuilder, BuilderContext>
{
    public PipelineBuilder(IEnumerable<IBuilderFeatureBuilder> builderFeatureBuilders)
    {
        AddFeatures(builderFeatureBuilders);
    }
}

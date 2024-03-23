namespace ClassFramework.Pipelines.Entity;

public class PipelineBuilder : PipelineBuilder<IConcreteTypeBuilder, EntityContext>
{
    public PipelineBuilder(IEnumerable<IEntityFeatureBuilder> entityFeatureBuilders)
    {
        AddFeatures(entityFeatureBuilders);
    }
}

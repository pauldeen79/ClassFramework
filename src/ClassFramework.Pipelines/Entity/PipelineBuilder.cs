namespace ClassFramework.Pipelines.Entity;

public class PipelineBuilder : PipelineBuilder<IConcreteTypeBuilder, EntityContext>
{
    public PipelineBuilder(
        IEnumerable<ISharedFeatureBuilder> sharedFeatureBuilders,
        IEnumerable<IEntityFeatureBuilder> entityFeatureBuilders)
    {
        AddFeatures(entityFeatureBuilders);
        AddFeatures(sharedFeatureBuilders.Select(x => x.BuildFor<EntityContext>()));
    }
}

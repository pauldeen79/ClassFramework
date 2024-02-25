namespace ClassFramework.Pipelines.OverrideEntity;

public class PipelineBuilder : PipelineBuilder<IConcreteTypeBuilder, OverrideEntityContext>
{
    public PipelineBuilder(
        IEnumerable<ISharedFeatureBuilder> sharedFeatureBuilders,
        IEnumerable<IOverrideEntityFeatureBuilder> entityFeatureBuilders)
    {
        AddFeatures(entityFeatureBuilders);
        AddFeatures(sharedFeatureBuilders.Select(x => x.BuildFor<OverrideEntityContext>()));
    }
}

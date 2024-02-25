namespace ClassFramework.Pipelines.BuilderExtension;

public class PipelineBuilder : PipelineBuilder<IConcreteTypeBuilder, BuilderExtensionContext>
{
    public PipelineBuilder(
        IEnumerable<ISharedFeatureBuilder> sharedFeatureBuilders,
        IEnumerable<IBuilderInterfaceFeatureBuilder> builderInterfaceFeatureBuilders)
    {
        AddFeatures(builderInterfaceFeatureBuilders);
        AddFeatures(sharedFeatureBuilders.Select(x => x.BuildFor<BuilderExtensionContext>()));
    }
}

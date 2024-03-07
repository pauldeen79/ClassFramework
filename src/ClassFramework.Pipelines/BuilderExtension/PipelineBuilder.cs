namespace ClassFramework.Pipelines.BuilderExtension;

public class PipelineBuilder : PipelineBuilder<IConcreteTypeBuilder, BuilderExtensionContext>
{
    public PipelineBuilder(
        IEnumerable<ISharedFeatureBuilder> sharedFeatureBuilders,
        IEnumerable<IBuilderExtensionFeatureBuilder> builderInterfaceFeatureBuilders)
    {
        AddFeatures(builderInterfaceFeatureBuilders);
        AddFeatures(sharedFeatureBuilders.Select(x => x.BuildFor<BuilderExtensionContext>()));
    }
}

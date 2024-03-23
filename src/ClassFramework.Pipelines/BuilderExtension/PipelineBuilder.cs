namespace ClassFramework.Pipelines.BuilderExtension;

public class PipelineBuilder : PipelineBuilder<IConcreteTypeBuilder, BuilderExtensionContext>
{
    public PipelineBuilder(IEnumerable<IBuilderExtensionFeatureBuilder> builderInterfaceFeatureBuilders)
    {
        AddFeatures(builderInterfaceFeatureBuilders);
    }
}

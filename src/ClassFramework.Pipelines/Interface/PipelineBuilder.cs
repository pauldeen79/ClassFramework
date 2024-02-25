namespace ClassFramework.Pipelines.Interface;

public class PipelineBuilder : PipelineBuilder<InterfaceBuilder, InterfaceContext>
{
    public PipelineBuilder(
        IEnumerable<IInterfaceFeatureBuilder> interfaceFeatureBuilders)
    {
        AddFeatures(interfaceFeatureBuilders);
    }
}

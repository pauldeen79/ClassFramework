namespace ClassFramework.Pipelines.BuilderExtension;

public class PipelineBuilder : PipelineBuilder<BuilderExtensionContext>
{
    public PipelineBuilder(IEnumerable<IBuilderExtensionComponentBuilder> builderInterfaceComponentBuilders)
    {
        AddComponents(builderInterfaceComponentBuilders);
    }
}

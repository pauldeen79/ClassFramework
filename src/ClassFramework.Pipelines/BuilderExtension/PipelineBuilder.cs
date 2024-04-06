namespace ClassFramework.Pipelines.BuilderExtension;

public class PipelineBuilder : PipelineBuilder<IConcreteTypeBuilder, BuilderExtensionContext>
{
    public PipelineBuilder(IEnumerable<IBuilderExtensionComponentBuilder> builderInterfaceComponentBuilders)
    {
        AddComponents(builderInterfaceComponentBuilders);
    }
}

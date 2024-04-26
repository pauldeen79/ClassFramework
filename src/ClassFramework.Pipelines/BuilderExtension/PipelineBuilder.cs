namespace ClassFramework.Pipelines.BuilderExtension;

public class PipelineBuilder : PipelineBuilder<BuilderExtensionContext, IConcreteTypeBuilder>
{
    public PipelineBuilder(IEnumerable<IBuilderExtensionComponentBuilder> builderInterfaceComponentBuilders)
    {
        AddComponents(builderInterfaceComponentBuilders);
    }
}

namespace ClassFramework.Pipelines.BuilderInterface;

public class PipelineBuilder : PipelineBuilder<BuilderInterfaceContext, InterfaceBuilder>
{
    public PipelineBuilder(IEnumerable<IBuilderInterfaceComponentBuilder> builderInterfaceComponentBuilders)
    {
        AddComponents(builderInterfaceComponentBuilders);
    }
}

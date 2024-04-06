namespace ClassFramework.Pipelines.Interface;

public class PipelineBuilder : PipelineBuilder<InterfaceBuilder, InterfaceContext>
{
    public PipelineBuilder(IEnumerable<IInterfaceComponentBuilder> interfaceComponentBuilders)
    {
        AddComponents(interfaceComponentBuilders);
    }
}

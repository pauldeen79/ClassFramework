namespace ClassFramework.Pipelines.Interface;

public class PipelineBuilder : PipelineBuilder<InterfaceContext>
{
    public PipelineBuilder(IEnumerable<IInterfaceComponentBuilder> interfaceComponentBuilders)
    {
        AddComponents(interfaceComponentBuilders);
    }
}

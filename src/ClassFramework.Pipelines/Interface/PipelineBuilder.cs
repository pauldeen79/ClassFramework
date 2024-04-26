namespace ClassFramework.Pipelines.Interface;

public class PipelineBuilder : PipelineBuilder<InterfaceContext, InterfaceBuilder>
{
    public PipelineBuilder(IEnumerable<IInterfaceComponentBuilder> interfaceComponentBuilders)
    {
        AddComponents(interfaceComponentBuilders);
    }
}

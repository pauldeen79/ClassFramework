namespace ClassFramework.Pipelines.Reflection;

public class PipelineBuilder : PipelineBuilder<ReflectionContext, TypeBaseBuilder>
{
    public PipelineBuilder(IEnumerable<IReflectionComponentBuilder> reflectionComponentBuilders)
    {
        AddComponents(reflectionComponentBuilders);
    }
}

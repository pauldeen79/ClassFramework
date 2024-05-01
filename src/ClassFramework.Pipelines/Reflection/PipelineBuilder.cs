namespace ClassFramework.Pipelines.Reflection;

public class PipelineBuilder : PipelineBuilder<ReflectionContext>
{
    public PipelineBuilder(IEnumerable<IReflectionComponentBuilder> reflectionComponentBuilders)
    {
        AddComponents(reflectionComponentBuilders);
    }
}

namespace ClassFramework.Pipelines.Reflection;

public class PipelineBuilder : PipelineBuilder<TypeBaseBuilder, ReflectionContext>
{
    public PipelineBuilder(IEnumerable<IReflectionComponentBuilder> reflectionComponentBuilders)
    {
        AddComponents(reflectionComponentBuilders);
    }
}

namespace ClassFramework.Pipelines.Builder;

public class PipelineBuilder : PipelineBuilder<BuilderContext, IConcreteTypeBuilder>
{
    public PipelineBuilder(IEnumerable<IBuilderComponentBuilder> builderComponentBuilders)
    {
        AddComponents(builderComponentBuilders);
    }
}

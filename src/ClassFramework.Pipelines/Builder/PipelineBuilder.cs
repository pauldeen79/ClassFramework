namespace ClassFramework.Pipelines.Builder;

public class PipelineBuilder : PipelineBuilder<IConcreteTypeBuilder, BuilderContext>
{
    public PipelineBuilder(IEnumerable<IBuilderComponentBuilder> builderComponentBuilders)
    {
        AddComponents(builderComponentBuilders);
    }
}

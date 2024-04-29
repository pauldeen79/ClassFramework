namespace ClassFramework.Pipelines.Builder;

public class PipelineBuilder : PipelineBuilder<BuilderContext>
{
    public PipelineBuilder(IEnumerable<IBuilderComponentBuilder> builderComponentBuilders)
    {
        AddComponents(builderComponentBuilders);
    }
}

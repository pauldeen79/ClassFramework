namespace ClassFramework.Pipelines.Entity;

public class PipelineBuilder : PipelineBuilder<IConcreteTypeBuilder, EntityContext>
{
    public PipelineBuilder(IEnumerable<IEntityComponentBuilder> entityComponentBuilders)
    {
        AddComponents(entityComponentBuilders);
    }
}

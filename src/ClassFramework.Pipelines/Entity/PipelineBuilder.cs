namespace ClassFramework.Pipelines.Entity;

public class PipelineBuilder : PipelineBuilder<EntityContext>
{
    public PipelineBuilder(IEnumerable<IEntityComponentBuilder> entityComponentBuilders)
    {
        AddComponents(entityComponentBuilders);
    }
}

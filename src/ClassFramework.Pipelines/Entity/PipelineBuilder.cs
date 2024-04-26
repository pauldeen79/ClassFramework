namespace ClassFramework.Pipelines.Entity;

public class PipelineBuilder : PipelineBuilder<EntityContext, IConcreteTypeBuilder>
{
    public PipelineBuilder(IEnumerable<IEntityComponentBuilder> entityComponentBuilders)
    {
        AddComponents(entityComponentBuilders);
    }
}

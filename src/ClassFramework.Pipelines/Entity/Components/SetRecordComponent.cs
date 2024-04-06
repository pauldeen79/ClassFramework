namespace ClassFramework.Pipelines.Entity.Features;

public class SetRecordComponentBuilder : IEntityComponentBuilder
{
    public IPipelineComponent<IConcreteTypeBuilder, EntityContext> Build()
        => new SetRecordComponent();
}

public class SetRecordComponent : IPipelineComponent<IConcreteTypeBuilder, EntityContext>
{
    public Result<IConcreteTypeBuilder> Process(PipelineContext<IConcreteTypeBuilder, EntityContext> context)
    {
        context = context.IsNotNull(nameof(context));

        if (context.Model is IRecordContainerBuilder recordContainerBuilder)
        {
            recordContainerBuilder.WithRecord(context.Context.Settings.CreateRecord);
        }

        return Result.Continue<IConcreteTypeBuilder>();
    }
}

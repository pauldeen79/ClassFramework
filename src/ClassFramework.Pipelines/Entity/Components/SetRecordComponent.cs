namespace ClassFramework.Pipelines.Entity.Features;

public class SetRecordComponentBuilder : IEntityComponentBuilder
{
    public IPipelineComponent<EntityContext, IConcreteTypeBuilder> Build()
        => new SetRecordComponent();
}

public class SetRecordComponent : IPipelineComponent<EntityContext, IConcreteTypeBuilder>
{
    public Task<Result> Process(PipelineContext<EntityContext, IConcreteTypeBuilder> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        if (context.Response is IRecordContainerBuilder recordContainerBuilder)
        {
            recordContainerBuilder.WithRecord(context.Request.Settings.CreateRecord);
        }

        return Task.FromResult(Result.Continue());
    }
}

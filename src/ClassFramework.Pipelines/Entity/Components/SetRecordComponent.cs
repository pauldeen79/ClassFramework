namespace ClassFramework.Pipelines.Entity.Components;

public class SetRecordComponentBuilder : IEntityComponentBuilder
{
    public IPipelineComponent<EntityContext> Build()
        => new SetRecordComponent();
}

public class SetRecordComponent : IPipelineComponent<EntityContext>
{
    public Task<Result> Process(PipelineContext<EntityContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        context.Request.Builder.WithRecord(context.Request.Settings.CreateRecord);

        return Task.FromResult(Result.Continue());
    }
}

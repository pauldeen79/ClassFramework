namespace ClassFramework.Pipelines.Entity.Components;

public class SetBaseClassComponent : IPipelineComponent<EntityContext>
{
    public Task<Result> ProcessAsync(PipelineContext<EntityContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        context.Request.Builder.WithBaseClass(context.Request.SourceModel.GetEntityBaseClass(context.Request.Settings.EnableInheritance, context.Request.Settings.BaseClass));

        return Task.FromResult(Result.Success());
    }
}

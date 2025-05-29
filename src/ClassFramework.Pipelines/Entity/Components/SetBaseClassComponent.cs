namespace ClassFramework.Pipelines.Entity.Components;

public class SetBaseClassComponent : IPipelineComponent<EntityContext>
{
    public async Task<Result> ProcessAsync(PipelineContext<EntityContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        context.Request.Builder.WithBaseClass(await context.Request.SourceModel.GetEntityBaseClassAsync(context.Request.Settings.EnableInheritance, context.Request.Settings.BaseClass).ConfigureAwait(false));

        return Result.Success();
    }
}

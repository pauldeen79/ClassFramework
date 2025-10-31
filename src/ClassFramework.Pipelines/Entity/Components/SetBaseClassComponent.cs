namespace ClassFramework.Pipelines.Entity.Components;

public class SetBaseClassComponent : IPipelineComponent<EntityContext>, IOrderContainer
{
    public int Order => PipelineStage.Process;

    public async Task<Result> ExecuteAsync(EntityContext context, ICommandService commandService, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        context.Builder.WithBaseClass(await context.SourceModel.GetEntityBaseClassAsync(context.Settings.EnableInheritance, context.Settings.BaseClass).ConfigureAwait(false));

        return Result.Success();
    }
}

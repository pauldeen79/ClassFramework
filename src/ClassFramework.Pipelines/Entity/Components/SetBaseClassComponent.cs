namespace ClassFramework.Pipelines.Entity.Components;

public class SetBaseClassComponent : IPipelineComponent<EntityContext, ClassBuilder>
{
    public async Task<Result> ExecuteAsync(EntityContext context, ClassBuilder response, ICommandService commandService, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        response.WithBaseClass(await context.SourceModel.GetEntityBaseClassAsync(context.Settings.EnableInheritance, context.Settings.BaseClass).ConfigureAwait(false));

        return Result.Success();
    }
}

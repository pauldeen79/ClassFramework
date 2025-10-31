namespace ClassFramework.Pipelines.Entity.Components;

public class AddInterfacesComponent : IPipelineComponent<EntityContext>, IOrderContainer
{
    public int Order => PipelineStage.Process;

    public async Task<Result> ExecuteAsync(EntityContext context, ICommandService commandService, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        if (!context.Settings.CopyInterfaces)
        {
            return Result.Continue();
        }

        var baseClass = await context.SourceModel.GetEntityBaseClassAsync(context.Settings.EnableInheritance, context.Settings.BaseClass).ConfigureAwait(false);

        context.Builder.AddInterfaces(context.SourceModel.Interfaces
            .Where(x => context.Settings.CopyInterfacePredicate?.Invoke(x) ?? true)
            .Where(x => x != baseClass)
            .Select(x => context.MapTypeName(x.FixTypeName()))
            .Where(x => !string.IsNullOrEmpty(x)));

        return Result.Success();
    }
}

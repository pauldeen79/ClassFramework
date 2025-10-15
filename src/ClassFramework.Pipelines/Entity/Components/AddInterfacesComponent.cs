namespace ClassFramework.Pipelines.Entity.Components;

public class AddInterfacesComponent : IPipelineComponent<EntityContext>, IOrderContainer
{
    public int Order => PipelineStage.Process;

    public async Task<Result> ProcessAsync(PipelineContext<EntityContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        if (!context.Request.Settings.CopyInterfaces)
        {
            return Result.Continue();
        }

        var baseClass = await context.Request.SourceModel.GetEntityBaseClassAsync(context.Request.Settings.EnableInheritance, context.Request.Settings.BaseClass).ConfigureAwait(false);

        context.Request.Builder.AddInterfaces(context.Request.SourceModel.Interfaces
            .Where(x => context.Request.Settings.CopyInterfacePredicate?.Invoke(x) ?? true)
            .Where(x => x != baseClass)
            .Select(x => context.Request.MapTypeName(x.FixTypeName()))
            .Where(x => !string.IsNullOrEmpty(x)));

        return Result.Success();
    }
}

namespace ClassFramework.Pipelines.Entity.Components;

public class AddInterfacesComponent : IPipelineComponent<EntityContext>
{
    public async Task<Result> ProcessAsync(PipelineContext<EntityContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        if (context.Request.Settings.UseCrossCuttingInterfaces)
        {
            context.Request.Builder.AddInterfaces(typeof(IBuildableEntity<object>).ReplaceGenericTypeName(context.Request.MapTypeName(context.Request.SourceModel.GetFullName())));
        }

        if (!context.Request.Settings.CopyInterfaces)
        {
            return Result.Success();
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

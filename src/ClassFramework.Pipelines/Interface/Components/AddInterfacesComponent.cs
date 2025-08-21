namespace ClassFramework.Pipelines.Interface.Components;

public class AddInterfacesComponent : IPipelineComponent<InterfaceContext>
{
    public Task<Result> ProcessAsync(PipelineContext<InterfaceContext> context, CancellationToken token)
        => Task.Run(() =>
        {
            context = context.IsNotNull(nameof(context));

            if (!context.Request.Settings.CopyInterfaces)
            {
                return Result.Continue();
            }

            context.Request.Builder.AddInterfaces(context.Request.SourceModel.Interfaces
                .Where(x => context.Request.Settings.CopyInterfacePredicate?.Invoke(x) ?? true)
                .Select(x => context.Request.MapTypeName(x.FixTypeName()))
                .Where(x => !string.IsNullOrEmpty(x)));

            return Result.Success();
        }, token);
}

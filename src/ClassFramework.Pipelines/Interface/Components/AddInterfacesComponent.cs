namespace ClassFramework.Pipelines.Interface.Components;

public class AddInterfacesComponent : IPipelineComponent<InterfaceContext>
{
    public Task<Result> ExecuteAsync(InterfaceContext context, ICommandService commandService, CancellationToken token)
        => Task.Run(() =>
        {
            context = context.IsNotNull(nameof(context));

            if (!context.Settings.CopyInterfaces)
            {
                return Result.Continue();
            }

            context.Builder.AddInterfaces(context.SourceModel.Interfaces
                .Where(x => context.Settings.CopyInterfacePredicate?.Invoke(x) ?? true)
                .Select(x => context.MapTypeName(x.FixTypeName()))
                .Where(x => !string.IsNullOrEmpty(x)));

            return Result.Success();
        }, token);
}

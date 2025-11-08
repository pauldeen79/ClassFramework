namespace ClassFramework.Pipelines.Interface.Components;

public class AddInterfacesComponent : IPipelineComponent<InterfaceContext, InterfaceBuilder>
{
    public Task<Result> ExecuteAsync(InterfaceContext context, InterfaceBuilder response, ICommandService commandService, CancellationToken token)
        => Task.Run(() =>
        {
            context = context.IsNotNull(nameof(context));
            response = response.IsNotNull(nameof(response));

            if (!context.Settings.CopyInterfaces)
            {
                return Result.Continue();
            }

            response.AddInterfaces(context.SourceModel.Interfaces
                .Where(x => context.Settings.CopyInterfacePredicate?.Invoke(x) ?? true)
                .Select(x => context.MapTypeName(x.FixTypeName()))
                .Where(x => !string.IsNullOrEmpty(x)));

            return Result.Success();
        }, token);
}

namespace ClassFramework.Pipelines.Reflection.Components;

public class AddInterfacesComponent : IPipelineComponent<ReflectionContext>
{
    public Task<Result> ExecuteAsync(ReflectionContext context, ICommandService commandService, CancellationToken token)
        => Task.Run(() =>
        {
            context = context.IsNotNull(nameof(context));

            if (!context.Settings.CopyInterfaces)
            {
                return Result.Continue();
            }

            context.Builder.AddInterfaces(
                context.SourceModel.GetInterfaces()
                    .Where(x => !(context.SourceModel.IsRecord() && x.FullName.StartsWith($"System.IEquatable`1[[{context.SourceModel.FullName}")))
                    .Select(x => context.GetMappedTypeName(x, context.SourceModel))
                    .Where(x => context.Settings.CopyInterfacePredicate?.Invoke(x) ?? true)
                    .Select(x => context.MapTypeName(x))
            );

            return Result.Success();
        }, token);
}

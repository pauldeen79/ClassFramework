namespace ClassFramework.Pipelines.Reflection.Components;

public class AddInterfacesComponent : IPipelineComponent<ReflectionContext>
{
    public Task<Result> ProcessAsync(PipelineContext<ReflectionContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        if (!context.Request.Settings.CopyInterfaces)
        {
            return Task.FromResult(Result.Success());
        }

        context.Request.Builder.AddInterfaces(
            context.Request.SourceModel.GetInterfaces()
                .Where(x => !(context.Request.SourceModel.IsRecord() && x.FullName.StartsWith($"System.IEquatable`1[[{context.Request.SourceModel.FullName}")))
                .Select(x => context.Request.GetMappedTypeName(x, context.Request.SourceModel))
                .Where(x => context.Request.Settings.CopyInterfacePredicate?.Invoke(x) ?? true)
                .Select(x => context.Request.MapTypeName(x, string.Empty))
        );

        return Task.FromResult(Result.Success());
    }
}

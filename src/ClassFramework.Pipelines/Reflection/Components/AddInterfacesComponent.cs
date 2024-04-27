namespace ClassFramework.Pipelines.Reflection.Components;

public class AddInterfacesComponentBuilder : IReflectionComponentBuilder
{
    public IPipelineComponent<ReflectionContext, TypeBaseBuilder> Build()
        => new AddInterfacesComponent();
}

public class AddInterfacesComponent : IPipelineComponent<ReflectionContext, TypeBaseBuilder>
{
    public Task<Result> Process(PipelineContext<ReflectionContext, TypeBaseBuilder> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        if (!context.Request.Settings.CopyInterfaces)
        {
            return Task.FromResult(Result.Continue());
        }

        context.Response.AddInterfaces(
            context.Request.SourceModel.GetInterfaces()
                .Where(x => !(context.Request.SourceModel.IsRecord() && x.FullName.StartsWith($"System.IEquatable`1[[{context.Request.SourceModel.FullName}")))
                .Select(x => context.Request.GetMappedTypeName(x, context.Request.SourceModel))
                .Where(x => context.Request.Settings.CopyInterfacePredicate?.Invoke(x) ?? true)
                .Select(x => context.Request.MapTypeName(x))
        );

        return Task.FromResult(Result.Continue());
    }
}

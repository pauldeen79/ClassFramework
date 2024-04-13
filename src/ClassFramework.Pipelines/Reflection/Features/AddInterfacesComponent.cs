namespace ClassFramework.Pipelines.Reflection.Features;

public class AddInterfacesComponentBuilder : IReflectionComponentBuilder
{
    public IPipelineComponent<TypeBaseBuilder, ReflectionContext> Build()
        => new AddInterfacesComponent();
}

public class AddInterfacesComponent : IPipelineComponent<TypeBaseBuilder, ReflectionContext>
{
    public Task<Result<TypeBaseBuilder>> Process(PipelineContext<TypeBaseBuilder, ReflectionContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        if (!context.Context.Settings.CopyInterfaces)
        {
            return Task.FromResult(Result.Continue<TypeBaseBuilder>());
        }

        context.Model.AddInterfaces(
            context.Context.SourceModel.GetInterfaces()
                .Where(x => !(context.Context.SourceModel.IsRecord() && x.FullName.StartsWith($"System.IEquatable`1[[{context.Context.SourceModel.FullName}")))
                .Select(x => context.Context.GetMappedTypeName(x, context.Context.SourceModel))
                .Where(x => context.Context.Settings.CopyInterfacePredicate?.Invoke(x) ?? true)
                .Select(x => context.Context.MapTypeName(x))
        );

        return Task.FromResult(Result.Continue<TypeBaseBuilder>());
    }
}

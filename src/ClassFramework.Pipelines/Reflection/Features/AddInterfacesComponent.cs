namespace ClassFramework.Pipelines.Reflection.Features;

public class AddInterfacesComponentBuilder : IReflectionComponentBuilder
{
    public IPipelineComponent<TypeBaseBuilder, ReflectionContext> Build()
        => new AddInterfacesComponent();
}

public class AddInterfacesComponent : IPipelineComponent<TypeBaseBuilder, ReflectionContext>
{
    public Result<TypeBaseBuilder> Process(PipelineContext<TypeBaseBuilder, ReflectionContext> context)
    {
        context = context.IsNotNull(nameof(context));

        if (!context.Context.Settings.CopyInterfaces)
        {
            return Result.Continue<TypeBaseBuilder>();
        }

        context.Model.AddInterfaces(
            context.Context.SourceModel.GetInterfaces()
                .Where(x => !(context.Context.SourceModel.IsRecord() && x.FullName.StartsWith($"System.IEquatable`1[[{context.Context.SourceModel.FullName}")))
                .Select(x => context.Context.GetMappedTypeName(x, context.Context.SourceModel))
                .Where(x => context.Context.Settings.CopyInterfacePredicate?.Invoke(x) ?? true)
                .Select(x => context.Context.MapTypeName(x))
        );

        return Result.Continue<TypeBaseBuilder>();
    }
}

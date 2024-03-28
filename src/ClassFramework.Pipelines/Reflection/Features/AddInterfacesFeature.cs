namespace ClassFramework.Pipelines.Reflection.Features;

public class AddInterfacesFeatureBuilder : IReflectionFeatureBuilder
{
    public IPipelineFeature<TypeBaseBuilder, ReflectionContext> Build()
        => new AddInterfacesFeature();
}

public class AddInterfacesFeature : IPipelineFeature<TypeBaseBuilder, ReflectionContext>
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
                .Select(x => x.GetTypeName(context.Context.SourceModel))
                .Where(x => context.Context.Settings.CopyInterfacePredicate?.Invoke(x) ?? true)
                .Select(x => context.Context.MapTypeName(x))
        );

        return Result.Continue<TypeBaseBuilder>();
    }

    public IBuilder<IPipelineFeature<TypeBaseBuilder, ReflectionContext>> ToBuilder()
        => new AddInterfacesFeatureBuilder();
}

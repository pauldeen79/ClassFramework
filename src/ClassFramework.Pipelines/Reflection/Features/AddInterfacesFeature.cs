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
                .Select(x => x.FullName.FixTypeName())
                .Where(x => context.Context.Settings.CopyInterfacePredicate?.Invoke(x) ?? true)
                .Select(n => context.Context.MapTypeName(n))
        );

        return Result.Continue<TypeBaseBuilder>();
    }

    public IBuilder<IPipelineFeature<TypeBaseBuilder, ReflectionContext>> ToBuilder()
        => new AddInterfacesFeatureBuilder();
}

namespace ClassFramework.Pipelines.Interface.Features;

public class AddInterfacesFeatureBuilder : IInterfaceFeatureBuilder
{
    public IPipelineFeature<InterfaceBuilder, InterfaceContext> Build()
        => new AddInterfacesFeature();
}

public class AddInterfacesFeature : IPipelineFeature<InterfaceBuilder, InterfaceContext>
{
    public Result<InterfaceBuilder> Process(PipelineContext<InterfaceBuilder, InterfaceContext> context)
    {
        context = context.IsNotNull(nameof(context));

        if (!context.Context.Settings.CopyInterfaces)
        {
            return Result.Continue<InterfaceBuilder>();
        }

        context.Model.AddInterfaces(context.Context.SourceModel.Interfaces
            .Where(x => context.Context.Settings.CopyInterfacePredicate?.Invoke(x) ?? true)
            .Select(x => context.Context.MapTypeName(x.FixTypeName()))
            .Where(x => !string.IsNullOrEmpty(x)));

        return Result.Continue<InterfaceBuilder>();
    }

    public IBuilder<IPipelineFeature<InterfaceBuilder, InterfaceContext>> ToBuilder()
        => new AddInterfacesFeatureBuilder();
}

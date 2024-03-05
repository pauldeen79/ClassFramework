namespace ClassFramework.Pipelines.Interface.Features;

public class AddMethodsFeatureBuilder : IInterfaceFeatureBuilder
{
    public IPipelineFeature<InterfaceBuilder, InterfaceContext> Build()
        => new AddMethodsFeature();
}

public class AddMethodsFeature : IPipelineFeature<InterfaceBuilder, InterfaceContext>
{
    public Result<InterfaceBuilder> Process(PipelineContext<InterfaceBuilder, InterfaceContext> context)
    {
        context = context.IsNotNull(nameof(context));

        if (!context.Context.Settings.CopyMethods)
        {
            return Result.Continue<InterfaceBuilder>();
        }

        context.Model.AddMethods(context.Context.SourceModel.Methods
            .Where(x => context.Context.Settings.CopyMethodPredicate is null || context.Context.Settings.CopyMethodPredicate(x))
            .Select(x => x.ToBuilder()));

        return Result.Continue<InterfaceBuilder>();
    }

    public IBuilder<IPipelineFeature<InterfaceBuilder, InterfaceContext>> ToBuilder()
        => new AddMethodsFeatureBuilder();
}

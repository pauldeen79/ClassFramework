namespace ClassFramework.Pipelines.OverrideEntity.Features;

public class ValidationFeatureBuilder : IOverrideEntityFeatureBuilder
{
    public IPipelineFeature<IConcreteTypeBuilder, OverrideEntityContext> Build() => new ValidationFeature();
}

public class ValidationFeature : IPipelineFeature<IConcreteTypeBuilder, OverrideEntityContext>
{
    public Result<IConcreteTypeBuilder> Process(PipelineContext<IConcreteTypeBuilder, OverrideEntityContext> context)
    {
        context = context.IsNotNull(nameof(context));

        if (!context.Context.Settings.AllowGenerationWithoutProperties
            && context.Context.SourceModel.Properties.Count == 0
            && !context.Context.Settings.EnableInheritance)
        {
            return Result.Invalid<IConcreteTypeBuilder>("To create an override entity class, there must be at least one property");
        }
        
        return Result.Continue<IConcreteTypeBuilder>();
    }

    public IBuilder<IPipelineFeature<IConcreteTypeBuilder, OverrideEntityContext>> ToBuilder()
        => new ValidationFeatureBuilder();
}

namespace ClassFramework.Pipelines.OverrideEntity.Features;

public class AddGenericsFeatureBuilder : IOverrideEntityFeatureBuilder
{
    public IPipelineFeature<IConcreteTypeBuilder, OverrideEntityContext> Build()
        => new AddGenericsFeature();
}

public class AddGenericsFeature : IPipelineFeature<IConcreteTypeBuilder, OverrideEntityContext>
{
    public Result<IConcreteTypeBuilder> Process(PipelineContext<IConcreteTypeBuilder, OverrideEntityContext> context)
    {
        context = context.IsNotNull(nameof(context));

        context.Model
            .AddGenericTypeArguments(context.Context.SourceModel.GenericTypeArguments)
            .AddGenericTypeArgumentConstraints(context.Context.SourceModel.GenericTypeArgumentConstraints);

        return Result.Continue<IConcreteTypeBuilder>();
    }

    public IBuilder<IPipelineFeature<IConcreteTypeBuilder, OverrideEntityContext>> ToBuilder()
        => new AddGenericsFeatureBuilder();
}

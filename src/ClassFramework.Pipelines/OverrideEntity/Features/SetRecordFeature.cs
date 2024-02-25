namespace ClassFramework.Pipelines.OverrideEntity.Features;

public class SetRecordFeatureBuilder : IOverrideEntityFeatureBuilder
{
    public IPipelineFeature<IConcreteTypeBuilder, OverrideEntityContext> Build()
        => new SetRecordFeature();
}

public class SetRecordFeature : IPipelineFeature<IConcreteTypeBuilder, OverrideEntityContext>
{
    public Result<IConcreteTypeBuilder> Process(PipelineContext<IConcreteTypeBuilder, OverrideEntityContext> context)
    {
        context = context.IsNotNull(nameof(context));

        if (context.Model is IRecordContainerBuilder recordContainerBuilder)
        {
            recordContainerBuilder.WithRecord(context.Context.Settings.CreateRecord);
        }

        return Result.Continue<IConcreteTypeBuilder>();
    }

    public IBuilder<IPipelineFeature<IConcreteTypeBuilder, OverrideEntityContext>> ToBuilder()
        => new SetRecordFeatureBuilder();
}

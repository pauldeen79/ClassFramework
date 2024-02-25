namespace ClassFramework.Pipelines.OverrideEntity.Features;

public class AddMetadataFeatureBuilder : IOverrideEntityFeatureBuilder
{
    public IPipelineFeature<IConcreteTypeBuilder, OverrideEntityContext> Build()
        => new AddMetadataFeature();
}

public class AddMetadataFeature : IPipelineFeature<IConcreteTypeBuilder, OverrideEntityContext>
{
    public Result<IConcreteTypeBuilder> Process(PipelineContext<IConcreteTypeBuilder, OverrideEntityContext> context)
    {
        context = context.IsNotNull(nameof(context));

        context.Model.AddMetadata(context.Context.SourceModel.Metadata.Select(x => x.ToBuilder()));

        return Result.Continue<IConcreteTypeBuilder>();
    }

    public IBuilder<IPipelineFeature<IConcreteTypeBuilder, OverrideEntityContext>> ToBuilder()
        => new AddMetadataFeatureBuilder();
}

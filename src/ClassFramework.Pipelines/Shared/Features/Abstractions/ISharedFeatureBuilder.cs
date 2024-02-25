namespace ClassFramework.Pipelines.Shared.Features.Abstractions;

public interface ISharedFeatureBuilder : IBuilder<IPipelineFeature<IConcreteTypeBuilder>>
{
    IBuilder<IPipelineFeature<IConcreteTypeBuilder, TContext>> BuildFor<TContext>();
}

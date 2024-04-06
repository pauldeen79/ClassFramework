namespace ClassFramework.Pipelines.Builder.Components.Abstractions;

public interface IBuilderComponentBuilder : IBuilder<IPipelineComponent<IConcreteTypeBuilder, BuilderContext>>
{
}

namespace ClassFramework.Pipelines.Builder.Features;

public class GenericsComponentBuilder : IBuilderComponentBuilder
{
    public IPipelineComponent<IConcreteTypeBuilder, BuilderContext> Build()
        => new GenericsComponent();
}

public class GenericsComponent : IPipelineComponent<IConcreteTypeBuilder, BuilderContext>
{
    public Result<IConcreteTypeBuilder> Process(PipelineContext<IConcreteTypeBuilder, BuilderContext> context)
    {
        context = context.IsNotNull(nameof(context));

        context.Model.AddGenericTypeArguments(context.Context.SourceModel.GenericTypeArguments);
        context.Model.AddGenericTypeArgumentConstraints(context.Context.SourceModel.GenericTypeArgumentConstraints);

        return Result.Continue<IConcreteTypeBuilder>();
    }
}

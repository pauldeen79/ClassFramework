namespace ClassFramework.Pipelines.BuilderExtension.Features;

public class SetStaticFeatureBuilder : IBuilderExtensionFeatureBuilder
{
    public IPipelineFeature<IConcreteTypeBuilder, BuilderExtensionContext> Build()
        => new SetStaticFeature();
}

public class SetStaticFeature : IPipelineFeature<IConcreteTypeBuilder, BuilderExtensionContext>
{
    public Result<IConcreteTypeBuilder> Process(PipelineContext<IConcreteTypeBuilder, BuilderExtensionContext> context)
    {
        context = context.IsNotNull(nameof(context));

        (context.Model as IReferenceTypeBuilder)?.WithStatic();

        return Result.Continue<IConcreteTypeBuilder>();
    }

    public IBuilder<IPipelineFeature<IConcreteTypeBuilder, BuilderExtensionContext>> ToBuilder()
        => new SetStaticFeatureBuilder();
}

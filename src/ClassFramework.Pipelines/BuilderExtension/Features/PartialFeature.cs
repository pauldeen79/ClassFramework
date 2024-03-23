namespace ClassFramework.Pipelines.BuilderExtension.Features;

public class PartialFeatureBuilder : IBuilderExtensionFeatureBuilder
{
    public IPipelineFeature<IConcreteTypeBuilder, BuilderExtensionContext> Build() => new PartialFeature();
}

public class PartialFeature : IPipelineFeature<IConcreteTypeBuilder, BuilderExtensionContext>
{
    public Result<IConcreteTypeBuilder> Process(PipelineContext<IConcreteTypeBuilder, BuilderExtensionContext> context)
    {
        context = context.IsNotNull(nameof(context));

        context.Model.WithPartial(context.Context.Settings.CreateAsPartial);

        return Result.Continue<IConcreteTypeBuilder>();
    }

    public IBuilder<IPipelineFeature<IConcreteTypeBuilder, BuilderExtensionContext>> ToBuilder()
        => new PartialFeatureBuilder();
}

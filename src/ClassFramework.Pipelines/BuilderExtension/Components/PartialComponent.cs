namespace ClassFramework.Pipelines.BuilderExtension.Components;

public class PartialComponentBuilder : IBuilderExtensionComponentBuilder
{
    public IPipelineComponent<IConcreteTypeBuilder, BuilderExtensionContext> Build() => new PartialComponent();
}

public class PartialComponent : IPipelineComponent<IConcreteTypeBuilder, BuilderExtensionContext>
{
    public Result<IConcreteTypeBuilder> Process(PipelineContext<IConcreteTypeBuilder, BuilderExtensionContext> context)
    {
        context = context.IsNotNull(nameof(context));

        context.Model.WithPartial(context.Context.Settings.CreateAsPartial);

        return Result.Continue<IConcreteTypeBuilder>();
    }
}

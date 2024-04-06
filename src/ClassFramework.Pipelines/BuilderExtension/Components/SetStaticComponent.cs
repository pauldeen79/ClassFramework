namespace ClassFramework.Pipelines.BuilderExtension.Components;

public class SetStaticComponentBuilder : IBuilderExtensionComponentBuilder
{
    public IPipelineComponent<IConcreteTypeBuilder, BuilderExtensionContext> Build()
        => new SetStaticComponent();
}

public class SetStaticComponent : IPipelineComponent<IConcreteTypeBuilder, BuilderExtensionContext>
{
    public Result<IConcreteTypeBuilder> Process(PipelineContext<IConcreteTypeBuilder, BuilderExtensionContext> context)
    {
        context = context.IsNotNull(nameof(context));

        (context.Model as IReferenceTypeBuilder)?.WithStatic();

        return Result.Continue<IConcreteTypeBuilder>();
    }
}

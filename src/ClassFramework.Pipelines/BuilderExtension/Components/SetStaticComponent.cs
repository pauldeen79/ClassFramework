namespace ClassFramework.Pipelines.BuilderExtension.Components;

public class SetStaticComponentBuilder : IBuilderExtensionComponentBuilder
{
    public IPipelineComponent<IConcreteTypeBuilder, BuilderExtensionContext> Build()
        => new SetStaticComponent();
}

public class SetStaticComponent : IPipelineComponent<IConcreteTypeBuilder, BuilderExtensionContext>
{
    public Task<Result<IConcreteTypeBuilder>> Process(PipelineContext<IConcreteTypeBuilder, BuilderExtensionContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        (context.Model as IReferenceTypeBuilder)?.WithStatic();

        return Task.FromResult(Result.Continue<IConcreteTypeBuilder>());
    }
}

namespace ClassFramework.Pipelines.BuilderExtension.Components;

public class SetStaticComponentBuilder : IBuilderExtensionComponentBuilder
{
    public IPipelineComponent<BuilderExtensionContext, IConcreteTypeBuilder> Build()
        => new SetStaticComponent();
}

public class SetStaticComponent : IPipelineComponent<BuilderExtensionContext, IConcreteTypeBuilder>
{
    public Task<Result> Process(PipelineContext<BuilderExtensionContext, IConcreteTypeBuilder> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        (context.Response as IReferenceTypeBuilder)?.WithStatic();

        return Task.FromResult(Result.Continue());
    }
}

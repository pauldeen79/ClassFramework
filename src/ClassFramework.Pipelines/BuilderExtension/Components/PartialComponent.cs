namespace ClassFramework.Pipelines.BuilderExtension.Components;

public class PartialComponentBuilder : IBuilderExtensionComponentBuilder
{
    public IPipelineComponent<IConcreteTypeBuilder, BuilderExtensionContext> Build() => new PartialComponent();
}

public class PartialComponent : IPipelineComponent<IConcreteTypeBuilder, BuilderExtensionContext>
{
    public Task<Result<IConcreteTypeBuilder>> Process(PipelineContext<IConcreteTypeBuilder, BuilderExtensionContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        context.Model.WithPartial(context.Context.Settings.CreateAsPartial);

        return Task.FromResult(Result.Continue<IConcreteTypeBuilder>());
    }
}

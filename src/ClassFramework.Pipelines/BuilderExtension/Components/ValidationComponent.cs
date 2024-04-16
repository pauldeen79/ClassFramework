namespace ClassFramework.Pipelines.BuilderExtension.Components;

public class ValidationComponentBuilder : IBuilderExtensionComponentBuilder
{
    public IPipelineComponent<IConcreteTypeBuilder, BuilderExtensionContext> Build() => new ValidationComponent();
}

public class ValidationComponent : IPipelineComponent<IConcreteTypeBuilder, BuilderExtensionContext>
{
    public Task<Result<IConcreteTypeBuilder>> Process(PipelineContext<IConcreteTypeBuilder, BuilderExtensionContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        if (!context.Context.Settings.AllowGenerationWithoutProperties
            && context.Context.SourceModel.Properties.Count == 0
            && !context.Context.Settings.EnableInheritance)
        {
            return Task.FromResult(Result.Invalid<IConcreteTypeBuilder>("To create a builder extensions class, there must be at least one property"));
        }
        
        return Task.FromResult(Result.Continue<IConcreteTypeBuilder>());
    }
}

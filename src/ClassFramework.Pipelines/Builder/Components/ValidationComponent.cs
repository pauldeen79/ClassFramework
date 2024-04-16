namespace ClassFramework.Pipelines.Builder.Features;

public class ValidationComponentBuilder : IBuilderComponentBuilder
{
    public IPipelineComponent<IConcreteTypeBuilder, BuilderContext> Build() => new ValidationComponent();
}

public class ValidationComponent : IPipelineComponent<IConcreteTypeBuilder, BuilderContext>
{
    public Task<Result<IConcreteTypeBuilder>> Process(PipelineContext<IConcreteTypeBuilder, BuilderContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        if (!context.Context.Settings.AllowGenerationWithoutProperties
            && context.Context.SourceModel.Properties.Count == 0
            && !context.Context.Settings.EnableInheritance)
        {
            return Task.FromResult(Result.Invalid<IConcreteTypeBuilder>("To create a builder class, there must be at least one property"));
        }
        
        return Task.FromResult(Result.Continue<IConcreteTypeBuilder>());
    }
}

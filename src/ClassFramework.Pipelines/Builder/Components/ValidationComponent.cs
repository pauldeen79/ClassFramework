namespace ClassFramework.Pipelines.Builder.Features;

public class ValidationComponentBuilder : IBuilderComponentBuilder
{
    public IPipelineComponent<BuilderContext, IConcreteTypeBuilder> Build() => new ValidationComponent();
}

public class ValidationComponent : IPipelineComponent<BuilderContext, IConcreteTypeBuilder>
{
    public Task<Result> Process(PipelineContext<BuilderContext, IConcreteTypeBuilder> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        if (!context.Request.Settings.AllowGenerationWithoutProperties
            && context.Request.SourceModel.Properties.Count == 0
            && !context.Request.Settings.EnableInheritance)
        {
            return Task.FromResult(Result.Invalid("To create a builder class, there must be at least one property"));
        }
        
        return Task.FromResult(Result.Continue());
    }
}

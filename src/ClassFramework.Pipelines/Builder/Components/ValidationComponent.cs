namespace ClassFramework.Pipelines.Builder.Components;

public class ValidationComponentBuilder : IBuilderComponentBuilder
{
    public IPipelineComponent<BuilderContext> Build() => new ValidationComponent();
}

public class ValidationComponent : IPipelineComponent<BuilderContext>
{
    public Task<Result> ProcessAsync(PipelineContext<BuilderContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        if (!context.Request.Settings.AllowGenerationWithoutProperties
            && context.Request.SourceModel.Properties.Count == 0
            && !context.Request.Settings.EnableInheritance)
        {
            return Task.FromResult(Result.Invalid("To create a builder class, there must be at least one property"));
        }

        return Task.FromResult(Result.Success());
    }
}

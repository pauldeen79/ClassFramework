namespace ClassFramework.Pipelines.Reflection.Features;

public class ValidationComponentBuilder : IReflectionComponentBuilder
{
    public IPipelineComponent<ReflectionContext, TypeBaseBuilder> Build() => new ValidationComponent();
}

public class ValidationComponent : IPipelineComponent<ReflectionContext, TypeBaseBuilder>
{
    public Task<Result> Process(PipelineContext<ReflectionContext, TypeBaseBuilder> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        if (!context.Request.Settings.AllowGenerationWithoutProperties
            && context.Request.SourceModel.GetProperties().Length == 0)
        {
            return Task.FromResult(Result.Invalid("To create a class, there must be at least one property"));
        }
        
        return Task.FromResult(Result.Continue());
    }
}

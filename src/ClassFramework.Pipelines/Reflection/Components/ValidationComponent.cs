namespace ClassFramework.Pipelines.Reflection.Components;

public class ValidationComponent : IPipelineComponent<ReflectionContext>
{
    public Task<Result> ProcessAsync(PipelineContext<ReflectionContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        if (!context.Request.Settings.AllowGenerationWithoutProperties
            && context.Request.SourceModel.GetProperties().Length == 0)
        {
            return Task.FromResult(Result.Invalid("To create a class, there must be at least one property"));
        }

        return Task.FromResult(Result.Success());
    }
}

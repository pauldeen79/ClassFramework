namespace ClassFramework.Pipelines.BuilderExtension.Components;

public class ValidationComponent : IPipelineComponent<BuilderExtensionContext>
{
    public Task<Result> ProcessAsync(PipelineContext<BuilderExtensionContext> context, CancellationToken token)
        => Task.Run(() =>
        {
            context = context.IsNotNull(nameof(context));

            if (!context.Request.Settings.AllowGenerationWithoutProperties
                && context.Request.SourceModel.Properties.Count == 0
                && !context.Request.Settings.EnableInheritance)
            {
                return Result.Invalid("To create a builder extensions class, there must be at least one property");
            }

            return Result.Success();
        }, token);
}

namespace ClassFramework.Pipelines.Reflection.Components;

public class ValidationComponent : IPipelineComponent<ReflectionContext>, IOrderContainer
{
    public int Order => PipelineStage.PreProcess;

    public Task<Result> ProcessAsync(PipelineContext<ReflectionContext> context, CancellationToken token)
        => Task.Run(() =>
        {
            context = context.IsNotNull(nameof(context));

            if (!context.Request.Settings.AllowGenerationWithoutProperties
                && context.Request.SourceModel.GetProperties().Length == 0)
            {
                return Result.Invalid("To create a class, there must be at least one property");
            }

            return Result.Success();
        }, token);
}

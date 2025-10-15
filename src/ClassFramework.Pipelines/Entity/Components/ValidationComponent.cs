namespace ClassFramework.Pipelines.Entity.Components;

public class ValidationComponent : IPipelineComponent<EntityContext>, IOrderContainer
{
    public int Order => PipelineStage.PreProcess;

    public Task<Result> ProcessAsync(PipelineContext<EntityContext> context, CancellationToken token)
        => Task.Run(() =>
        {
            context = context.IsNotNull(nameof(context));

            if (!context.Request.Settings.AllowGenerationWithoutProperties
                && context.Request.SourceModel.Properties.Count == 0
                && !context.Request.Settings.EnableInheritance)
            {
                return Result.Invalid("To create an entity class, there must be at least one property");
            }

            return Result.Success();
        }, token);
}

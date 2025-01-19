namespace ClassFramework.Pipelines.Entity.Components;

public class ValidationComponentBuilder : IEntityComponentBuilder
{
    public IPipelineComponent<EntityContext> Build() => new ValidationComponent();
}

public class ValidationComponent : IPipelineComponent<EntityContext>
{
    public Task<Result> ProcessAsync(PipelineContext<EntityContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        if (!context.Request.Settings.AllowGenerationWithoutProperties
            && context.Request.SourceModel.Properties.Count == 0
            && !context.Request.Settings.EnableInheritance)
        {
            return Task.FromResult(Result.Invalid("To create an entity class, there must be at least one property"));
        }

        return Task.FromResult(Result.Success());
    }
}

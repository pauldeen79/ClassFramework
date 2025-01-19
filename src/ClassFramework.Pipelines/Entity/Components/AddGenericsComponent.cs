namespace ClassFramework.Pipelines.Entity.Components;

public class AddGenericsComponent : IPipelineComponent<EntityContext>
{
    public Task<Result> ProcessAsync(PipelineContext<EntityContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        context.Request.Builder
            .AddGenericTypeArguments(context.Request.SourceModel.GenericTypeArguments)
            .AddGenericTypeArgumentConstraints(context.Request.SourceModel.GenericTypeArgumentConstraints);

        return Task.FromResult(Result.Success());
    }
}

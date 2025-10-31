namespace ClassFramework.Pipelines.Entity.Components;

public class AddGenericsComponent : IPipelineComponent<EntityContext>, IOrderContainer
{
    public int Order => PipelineStage.Process;

    public Task<Result> ExecuteAsync(EntityContext context, ICommandService commandService, CancellationToken token)
        => Task.Run(() =>
        {
            context = context.IsNotNull(nameof(context));

            context.Builder
                .AddGenericTypeArguments(context.SourceModel.GenericTypeArguments)
                .AddGenericTypeArgumentConstraints(context.SourceModel.GenericTypeArgumentConstraints);

            return Result.Success();
        }, token);
}

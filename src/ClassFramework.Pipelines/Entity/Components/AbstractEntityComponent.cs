namespace ClassFramework.Pipelines.Entity.Components;

public class AbstractEntityComponent : IPipelineComponent<EntityContext, ClassBuilder>
{
    public Task<Result> ExecuteAsync(EntityContext context, ClassBuilder response, ICommandService commandService, CancellationToken token)
        => Task.Run(() =>
        {
            context = context.IsNotNull(nameof(context));
            response = response.IsNotNull(nameof(response));

            response.WithAbstract(context.IsAbstract);

            return Result.Success();
        }, token);
}

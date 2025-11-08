namespace ClassFramework.Pipelines.Builder.Components;

public class GenericsComponent : IPipelineComponent<BuilderContext, ClassBuilder>
{
    public Task<Result> ExecuteAsync(BuilderContext context, ClassBuilder response, ICommandService commandService, CancellationToken token)
        => Task.Run(() =>
        {
            context = context.IsNotNull(nameof(context));
            response = response.IsNotNull(nameof(response));

            response.AddGenericTypeArguments(context.SourceModel.GenericTypeArguments);
            response.AddGenericTypeArgumentConstraints(context.SourceModel.GenericTypeArgumentConstraints);

            return Result.Success();
        }, token);
}

namespace ClassFramework.Pipelines.Reflection.Components;

public class SetBaseClassComponent : IPipelineComponent<ReflectionContext, TypeBaseBuilder>
{
    public Task<Result> ExecuteAsync(ReflectionContext context, TypeBaseBuilder response, ICommandService commandService, CancellationToken token)
        => Task.Run(() =>
        {
            context = context.IsNotNull(nameof(context));
            response = response.IsNotNull(nameof(response));

            if (response is IBaseClassContainerBuilder baseClassContainerBuilder)
            {
                baseClassContainerBuilder.WithBaseClass(context.SourceModel.GetEntityBaseClass(context.Settings));
            }

            return Result.Success();
        }, token);
}

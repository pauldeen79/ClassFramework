namespace ClassFramework.Pipelines.Reflection.Components;

public class SetBaseClassComponent : IPipelineComponent<ReflectionContext>, IOrderContainer
{
    public int Order => PipelineStage.Process;

    public Task<Result> ExecuteAsync(ReflectionContext context, ICommandService commandService, CancellationToken token)
        => Task.Run(() =>
        {
            context = context.IsNotNull(nameof(context));

            if (context.Builder is IBaseClassContainerBuilder baseClassContainerBuilder)
            {
                baseClassContainerBuilder.WithBaseClass(context.SourceModel.GetEntityBaseClass(context.Settings));
            }

            return Result.Success();
        }, token);
}

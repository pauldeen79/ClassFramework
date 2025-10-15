namespace ClassFramework.Pipelines.Reflection.Components;

public class SetBaseClassComponent : IPipelineComponent<ReflectionContext>, IOrderContainer
{
    public int Order => PipelineStage.Process;

    public Task<Result> ProcessAsync(PipelineContext<ReflectionContext> context, CancellationToken token)
        => Task.Run(() =>
        {
            context = context.IsNotNull(nameof(context));

            if (context.Request.Builder is IBaseClassContainerBuilder baseClassContainerBuilder)
            {
                baseClassContainerBuilder.WithBaseClass(context.Request.SourceModel.GetEntityBaseClass(context.Request.Settings));
            }

            return Result.Success();
        }, token);
}

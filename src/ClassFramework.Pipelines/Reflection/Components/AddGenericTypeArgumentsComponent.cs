namespace ClassFramework.Pipelines.Reflection.Components;

public class AddGenericTypeArgumentsComponent : IPipelineComponent<ReflectionContext>, IOrderContainer
{
    public int Order => PipelineStage.Process;

    public Task<Result> ProcessAsync(PipelineContext<ReflectionContext> context, CancellationToken token)
        => Task.Run(() =>
        {
            context = context.IsNotNull(nameof(context));

            context.Request.Builder.AddGenericTypeArguments(context.Request.SourceModel.GetGenericTypeArgumentTypeNames());

            return Result.Success();
        }, token);
}

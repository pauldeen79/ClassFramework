namespace ClassFramework.Pipelines.Reflection.Components;

public class AddGenericTypeArgumentsComponent : IPipelineComponent<ReflectionContext>, IOrderContainer
{
    public int Order => PipelineStage.Process;

    public Task<Result> ExecuteAsync(ReflectionContext context, ICommandService commandService, CancellationToken token)
        => Task.Run(() =>
        {
            context = context.IsNotNull(nameof(context));

            context.Builder.AddGenericTypeArguments(context.SourceModel.GetGenericTypeArgumentTypeNames());

            return Result.Success();
        }, token);
}

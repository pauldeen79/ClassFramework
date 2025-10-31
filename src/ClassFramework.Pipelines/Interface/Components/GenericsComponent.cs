namespace ClassFramework.Pipelines.Interface.Components;

public class GenericsComponent : IPipelineComponent<InterfaceContext>, IOrderContainer
{
    public int Order => PipelineStage.Process;

    public Task<Result> ExecuteAsync(InterfaceContext context, ICommandService commandService, CancellationToken token)
        => Task.Run(() =>
        {
            context = context.IsNotNull(nameof(context));

            context.Builder.AddGenericTypeArguments(context.SourceModel.GenericTypeArguments);
            context.Builder.AddGenericTypeArgumentConstraints(context.SourceModel.GenericTypeArgumentConstraints);

            return Result.Success();
        }, token);
}

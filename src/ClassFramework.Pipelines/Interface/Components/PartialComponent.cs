namespace ClassFramework.Pipelines.Interface.Components;

public class PartialComponent : IPipelineComponent<InterfaceContext>
{
    public Task<Result> ExecuteAsync(InterfaceContext context, ICommandService commandService, CancellationToken token)
        => Task.Run(() =>
        {
            context = context.IsNotNull(nameof(context));

            context.Builder.WithPartial(context.Settings.CreateAsPartial);

            return Result.Success();
        }, token);
}

namespace ClassFramework.Pipelines.Interface.Components;

public class PartialComponent : IPipelineComponent<InterfaceContext>
{
    public Task<Result> ProcessAsync(PipelineContext<InterfaceContext> context, CancellationToken token)
        => Task.Run(() =>
        {
            context = context.IsNotNull(nameof(context));

            context.Request.Builder.WithPartial(context.Request.Settings.CreateAsPartial);

            return Result.Success();
        }, token);
}

namespace ClassFramework.Pipelines.Interface.Components;

public class PartialComponent : IPipelineComponent<InterfaceContext, InterfaceBuilder>
{
    public Task<Result> ExecuteAsync(InterfaceContext context, InterfaceBuilder response, ICommandService commandService, CancellationToken token)
        => Task.Run(() =>
        {
            context = context.IsNotNull(nameof(context));

            response.WithPartial(context.Settings.CreateAsPartial);

            return Result.Success();
        }, token);
}

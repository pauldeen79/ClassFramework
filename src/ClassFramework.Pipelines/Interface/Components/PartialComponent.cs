namespace ClassFramework.Pipelines.Interface.Components;

public class PartialComponent : IPipelineComponent<GenerateInterfaceCommand, InterfaceBuilder>
{
    public Task<Result> ExecuteAsync(GenerateInterfaceCommand context, InterfaceBuilder response, ICommandService commandService, CancellationToken token)
        => Task.Run(() =>
        {
            context = context.IsNotNull(nameof(context));
            response = response.IsNotNull(nameof(response));

            response.WithPartial(context.Settings.CreateAsPartial);

            return Result.Success();
        }, token);
}

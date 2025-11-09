namespace ClassFramework.Pipelines.Builder.Components;

public class PartialComponent : IPipelineComponent<GenerateBuilderCommand, ClassBuilder>
{
    public Task<Result> ExecuteAsync(GenerateBuilderCommand context, ClassBuilder response, ICommandService commandService, CancellationToken token)
        => Task.Run(() =>
        {
            context = context.IsNotNull(nameof(context));
            response = response.IsNotNull(nameof(response));

            response.WithPartial(context.Settings.CreateAsPartial);

            return Result.Success();
        }, token);
}

namespace ClassFramework.Pipelines.Entity.Components;

public class PartialComponent : IPipelineComponent<GenerateEntityCommand, ClassBuilder>
{
    public Task<Result> ExecuteAsync(GenerateEntityCommand context, ClassBuilder response, ICommandService commandService, CancellationToken token)
        => Task.Run(() =>
        {
            context = context.IsNotNull(nameof(context));
            response = response.IsNotNull(nameof(response));

            response.WithPartial(context.Settings.CreateAsPartial);

            return Result.Success();
        }, token);
}

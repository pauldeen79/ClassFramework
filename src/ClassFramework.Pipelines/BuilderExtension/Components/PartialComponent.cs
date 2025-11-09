namespace ClassFramework.Pipelines.BuilderExtension.Components;

public class PartialComponent : IPipelineComponent<GenerateBuilderExtensionCommand, ClassBuilder>
{
    public Task<Result> ExecuteAsync(GenerateBuilderExtensionCommand context, ClassBuilder response, ICommandService commandService, CancellationToken token)
        => Task.Run(() =>
        {
            context = context.IsNotNull(nameof(context));
            response = response.IsNotNull(nameof(response));

            response.WithPartial(context.Settings.CreateAsPartial);

            return Result.Success();
        }, token);
}

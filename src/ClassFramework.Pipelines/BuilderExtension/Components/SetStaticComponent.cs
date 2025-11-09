namespace ClassFramework.Pipelines.BuilderExtension.Components;

public class SetStaticComponent : IPipelineComponent<GenerateBuilderExtensionCommand, ClassBuilder>
{
    public Task<Result> ExecuteAsync(GenerateBuilderExtensionCommand context, ClassBuilder response, ICommandService commandService, CancellationToken token)
        => Task.Run(() =>
        {
            context = context.IsNotNull(nameof(context));
            response = response.IsNotNull(nameof(response));

            response.WithStatic();

            return Result.Success();
        }, token);
}

namespace ClassFramework.Pipelines.Reflection.Components;

public class SetVisibilityComponent : IPipelineComponent<GenerateTypeFromReflectionCommand, TypeBaseBuilder>
{
    public Task<Result> ExecuteAsync(GenerateTypeFromReflectionCommand context, TypeBaseBuilder response, ICommandService commandService, CancellationToken token)
        => Task.Run(() =>
        {
            context = context.IsNotNull(nameof(context));
            response = response.IsNotNull(nameof(response));

            response.WithVisibility(GetVisibility(context));

            return Result.Success();
        }, token);

    private static Visibility GetVisibility(GenerateTypeFromReflectionCommand context)
    {
        if (context.SourceModel.IsPublic)
        {
            return Visibility.Public;
        }

        return context.SourceModel.IsNotPublic
            ? Visibility.Internal
            : Visibility.Private;
    }
}

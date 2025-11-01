namespace ClassFramework.Pipelines.Reflection.Components;

public class SetVisibilityComponent : IPipelineComponent<ReflectionContext>
{
    public Task<Result> ExecuteAsync(ReflectionContext context, ICommandService commandService, CancellationToken token)
        => Task.Run(() =>
        {
            context = context.IsNotNull(nameof(context));

            context.Builder.WithVisibility(GetVisibility(context));

            return Result.Success();
        }, token);

    private static Visibility GetVisibility(ReflectionContext context)
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

namespace ClassFramework.Pipelines.Reflection.Components;

public class SetVisibilityComponent : IPipelineComponent<ReflectionContext, TypeBaseBuilder>
{
    public Task<Result> ExecuteAsync(ReflectionContext context, TypeBaseBuilder response, ICommandService commandService, CancellationToken token)
        => Task.Run(() =>
        {
            context = context.IsNotNull(nameof(context));
            response = response.IsNotNull(nameof(response));

            response.WithVisibility(GetVisibility(context));

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

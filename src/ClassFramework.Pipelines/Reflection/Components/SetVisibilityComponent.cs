namespace ClassFramework.Pipelines.Reflection.Components;

public class SetVisibilityComponent : IPipelineComponent<ReflectionContext>
{
    public Task<Result> ProcessAsync(PipelineContext<ReflectionContext> context, CancellationToken token)
        => Task.Run(() =>
        {
            context = context.IsNotNull(nameof(context));

            context.Request.Builder.WithVisibility(GetVisibility(context));

            return Result.Success();
        }, token);

    private static Visibility GetVisibility(PipelineContext<ReflectionContext> context)
    {
        if (context.Request.SourceModel.IsPublic)
        {
            return Visibility.Public;
        }

        return context.Request.SourceModel.IsNotPublic
            ? Visibility.Internal
            : Visibility.Private;
    }
}

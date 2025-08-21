namespace ClassFramework.Pipelines.Reflection.Components;

public class SetVisibilityComponent : IPipelineComponent<ReflectionContext>
{
    public Task<Result> ProcessAsync(PipelineContext<ReflectionContext> context, CancellationToken token)
        => Task.Run(() =>
        {
            context = context.IsNotNull(nameof(context));

            if (context.Request.SourceModel.IsPublic)
            {
                context.Request.Builder.WithVisibility(Visibility.Public);
            }
            else
            {
                context.Request.Builder.WithVisibility(context.Request.SourceModel.IsNotPublic
                    ? Visibility.Internal
                    : Visibility.Private);
            }

            return Result.Success();
        }, token);
}

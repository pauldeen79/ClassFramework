namespace ClassFramework.Pipelines.Reflection.Components;

public class SetVisibilityComponentBuilder : IReflectionComponentBuilder
{
    public IPipelineComponent<ReflectionContext> Build()
        => new SetVisibilityComponent();
}

public class SetVisibilityComponent : IPipelineComponent<ReflectionContext>
{
    public Task<Result> Process(PipelineContext<ReflectionContext> context, CancellationToken token)
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

        return Task.FromResult(Result.Success());
    }
}

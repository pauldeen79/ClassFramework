namespace ClassFramework.Pipelines.Reflection.Components;

public class SetVisibilityComponentBuilder : IReflectionComponentBuilder
{
    public IPipelineComponent<ReflectionContext, TypeBaseBuilder> Build()
        => new SetVisibilityComponent();
}

public class SetVisibilityComponent : IPipelineComponent<ReflectionContext, TypeBaseBuilder>
{
    public Task<Result> Process(PipelineContext<ReflectionContext, TypeBaseBuilder> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        if (context.Request.SourceModel.IsPublic)
        {
            context.Response.WithVisibility(Visibility.Public);
        }
        else
        {
            context.Response.WithVisibility(context.Request.SourceModel.IsNotPublic
                ? Visibility.Internal
                : Visibility.Private);
        }

        return Task.FromResult(Result.Continue());
    }
}

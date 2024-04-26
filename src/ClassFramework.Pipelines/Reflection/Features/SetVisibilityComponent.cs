namespace ClassFramework.Pipelines.Reflection.Features;

public class SetVisibilityComponentBuilder : IReflectionComponentBuilder
{
    public IPipelineComponent<TypeBaseBuilder, ReflectionContext> Build()
        => new SetVisibilityComponent();
}

public class SetVisibilityComponent : IPipelineComponent<TypeBaseBuilder, ReflectionContext>
{
    public Task<Result<TypeBaseBuilder>> Process(PipelineContext<TypeBaseBuilder, ReflectionContext> context, CancellationToken token)
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

        return Task.FromResult(Result.Continue<TypeBaseBuilder>());
    }
}

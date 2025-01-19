namespace ClassFramework.Pipelines.Reflection.Components;

public class AddGenericTypeArgumentsComponentBuilder : IReflectionComponentBuilder
{
    public IPipelineComponent<ReflectionContext> Build()
        => new AddGenericTypeArgumentsComponent();
}

public class AddGenericTypeArgumentsComponent : IPipelineComponent<ReflectionContext>
{
    public Task<Result> ProcessAsync(PipelineContext<ReflectionContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        context.Request.Builder.AddGenericTypeArguments(context.Request.SourceModel.GetGenericTypeArgumentTypeNames());

        return Task.FromResult(Result.Success());
    }
}

namespace ClassFramework.Pipelines.Reflection.Components;

public class AddGenericTypeArgumentsComponentBuilder : IReflectionComponentBuilder
{
    public IPipelineComponent<ReflectionContext, TypeBaseBuilder> Build()
        => new AddGenericTypeArgumentsComponent();
}

public class AddGenericTypeArgumentsComponent : IPipelineComponent<ReflectionContext, TypeBaseBuilder>
{
    public Task<Result> Process(PipelineContext<ReflectionContext, TypeBaseBuilder> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        context.Response.AddGenericTypeArguments(context.Request.SourceModel.GetGenericTypeArgumentTypeNames());

        return Task.FromResult(Result.Continue());
    }
}

namespace ClassFramework.Pipelines.Interface.Components;

public class GenericsComponent : IPipelineComponent<InterfaceContext>
{
    public Task<Result> ProcessAsync(PipelineContext<InterfaceContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        context.Request.Builder.AddGenericTypeArguments(context.Request.SourceModel.GenericTypeArguments);
        context.Request.Builder.AddGenericTypeArgumentConstraints(context.Request.SourceModel.GenericTypeArgumentConstraints);

        return Task.FromResult(Result.Success());
    }
}

namespace ClassFramework.Pipelines.Builder.Components;

public class AddAttributesComponent : IPipelineComponent<BuilderContext>
{
    public Task<Result> ProcessAsync(PipelineContext<BuilderContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        context.Request.Builder.AddAttributes(context.Request.GetAtributes(context.Request.SourceModel.Attributes));

        return Task.FromResult(Result.Success());
    }
}

namespace ClassFramework.Pipelines.Entity.Components;

public class AddAttributesComponent : IPipelineComponent<EntityContext>, IOrderContainer
{
    public int Order => PipelineStage.Process;

    public Task<Result> ProcessAsync(PipelineContext<EntityContext> context, CancellationToken token)
        => Task.Run(() =>
        {
            context = context.IsNotNull(nameof(context));

            context.Request.Builder.AddAttributes(context.Request.GetAtributes(context.Request.SourceModel.Attributes));

            return Result.Success();
        }, token);
}

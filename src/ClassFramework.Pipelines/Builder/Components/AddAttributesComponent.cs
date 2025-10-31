namespace ClassFramework.Pipelines.Builder.Components;

public class AddAttributesComponent : IPipelineComponent<BuilderContext>, IOrderContainer
{
    public int Order => PipelineStage.Process;

    public Task<Result> ExecuteAsync(BuilderContext context, ICommandService commandService, CancellationToken token)
        => Task.Run(() =>
        {
            context = context.IsNotNull(nameof(context));

            context.Builder.AddAttributes(context.GetAtributes(context.SourceModel.Attributes));

            return Result.Success();
        }, token);
}

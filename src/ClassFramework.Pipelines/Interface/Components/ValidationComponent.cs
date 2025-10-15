namespace ClassFramework.Pipelines.Interface.Components;

public class ValidationComponent : IPipelineComponent<InterfaceContext>, IOrderContainer
{
    public int Order => PipelineStage.PreProcess;

    public Task<Result> ProcessAsync(PipelineContext<InterfaceContext> context, CancellationToken token)
        => Task.Run(() =>
        {
            context = context.IsNotNull(nameof(context));

            if (!context.Request.Settings.AllowGenerationWithoutProperties
                && context.Request.SourceModel.Properties.Count == 0)
            {
                return Result.Invalid("To create an interface, there must be at least one property");
            }

            return Result.Success();
        }, token);
}

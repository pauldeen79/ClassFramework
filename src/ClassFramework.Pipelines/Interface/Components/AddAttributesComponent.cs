namespace ClassFramework.Pipelines.Interface.Components;

public class AddAttributesComponent : IPipelineComponent<InterfaceContext>, IOrderContainer
{
    public int Order => PipelineStage.Process;

    public Task<Result> ProcessAsync(PipelineContext<InterfaceContext> context, CancellationToken token)
        => Task.Run(() =>
        {
            context = context.IsNotNull(nameof(context));

            if (!context.Request.Settings.CopyAttributes)
            {
                return Result.Continue();
            }

            context.Request.Builder.AddAttributes(context.Request.SourceModel.Attributes
                .Where(x => context.Request.Settings.CopyAttributePredicate?.Invoke(x) ?? true)
                .Select(x => context.Request.MapAttribute(x).ToBuilder()));

            return Result.Success();
        }, token);
}

namespace ClassFramework.Pipelines.Builder.Components;

public class AddInterfacesComponent(IExpressionEvaluator evaluator) : IPipelineComponent<BuilderContext>, IOrderContainer
{
    private readonly IExpressionEvaluator _evaluator = evaluator.IsNotNull(nameof(evaluator));

    public int Order => PipelineStage.Process;

    public async Task<Result> ProcessAsync(PipelineContext<BuilderContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        if (!context.Request.Settings.CopyInterfaces)
        {
            return Result.Continue();
        }

        var interfaces = await context.Request.GetInterfaceResultsAsync(
            (_, x) => x.ToString(),
            x => context.Request.MapTypeName(x.FixTypeName()),
            _evaluator,
            true,
            token).ConfigureAwait(false);

        var error = Array.Find(interfaces, x => !x.IsSuccessful());
        if (error is not null)
        {
            return error;
        }

        context.Request.Builder.AddInterfaces(interfaces.Select(x => x.Value!));

        return Result.Success();
    }
}

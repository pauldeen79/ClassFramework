namespace ClassFramework.Pipelines.Builder.Components;

public class AddInterfacesComponent(IExpressionEvaluator evaluator) : IPipelineComponent<BuilderContext>
{
    private readonly IExpressionEvaluator _evaluator = evaluator.IsNotNull(nameof(evaluator));

    public Task<Result> ProcessAsync(PipelineContext<BuilderContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        if (!context.Request.Settings.CopyInterfaces)
        {
            return Task.FromResult(Result.Success());
        }

        var interfaces = context.Request.GetInterfaceResults(
            (_, x) => x.ToString(),
            x => context.Request.MapTypeName(x.FixTypeName()),
            _evaluator,
            true);

        var error = Array.Find(interfaces, x => !x.IsSuccessful());
        if (error is not null)
        {
            return Task.FromResult<Result>(error);
        }

        context.Request.Builder.AddInterfaces(interfaces.Select(x => x.Value!));

        return Task.FromResult(Result.Success());
    }
}

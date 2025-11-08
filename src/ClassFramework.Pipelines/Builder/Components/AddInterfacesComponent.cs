namespace ClassFramework.Pipelines.Builder.Components;

public class AddInterfacesComponent(IExpressionEvaluator evaluator) : IPipelineComponent<BuilderContext, ClassBuilder>
{
    private readonly IExpressionEvaluator _evaluator = evaluator.IsNotNull(nameof(evaluator));

    public async Task<Result> ExecuteAsync(BuilderContext context, ClassBuilder response, ICommandService commandService, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));
        response = response.IsNotNull(nameof(response));

        if (!context.Settings.CopyInterfaces)
        {
            return Result.Continue();
        }

        var interfaces = await context.GetInterfaceResultsAsync(
            (_, x) => x.ToString(),
            x => context.MapTypeName(x.FixTypeName()),
            _evaluator,
            true,
            token).ConfigureAwait(false);

        var error = Array.Find(interfaces, x => !x.IsSuccessful());
        if (error is not null)
        {
            return error;
        }

        response.AddInterfaces(interfaces.Select(x => x.Value!));

        return Result.Success();
    }
}

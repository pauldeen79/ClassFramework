namespace ClassFramework.Pipelines.Builder.Components;

public class AddInterfacesComponent(IFormattableStringParser formattableStringParser) : IPipelineComponent<BuilderContext>
{
    private readonly IFormattableStringParser _formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));

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
            _formattableStringParser,
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

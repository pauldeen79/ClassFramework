namespace ClassFramework.Pipelines.BuilderExtension.Components;

public class SetNameComponent(IFormattableStringParser formattableStringParser) : IPipelineComponent<BuilderExtensionContext>
{
    private readonly IFormattableStringParser _formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));

    public Task<Result> ProcessAsync(PipelineContext<BuilderExtensionContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        var results = new ResultDictionaryBuilder<GenericFormattableString>()
            .Add(NamedResults.Name, () => _formattableStringParser.Parse(context.Request.Settings.BuilderExtensionsNameFormatString, context.Request.FormatProvider, context.Request))
            .Add(NamedResults.Namespace, () => _formattableStringParser.Parse(context.Request.Settings.BuilderExtensionsNamespaceFormatString, context.Request.FormatProvider, context.Request))
            .Build();

        var error = results.GetError();
        if (error is not null)
        {
            // Error in formattable string parsing
            return Task.FromResult<Result>(error);
        }

        context.Request.Builder
            .WithName(results[NamedResults.Name].Value!)
            .WithNamespace(results[NamedResults.Namespace].Value!);

        return Task.FromResult(Result.Success());
    }
}

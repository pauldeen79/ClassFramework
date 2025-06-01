namespace ClassFramework.Pipelines.BuilderExtension.Components;

public class SetNameComponent(IExpressionEvaluator evaluator) : IPipelineComponent<BuilderExtensionContext>
{
    private readonly IExpressionEvaluator _evaluator = evaluator.IsNotNull(nameof(evaluator));

    public async Task<Result> ProcessAsync(PipelineContext<BuilderExtensionContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        var results = await new AsyncResultDictionaryBuilder<GenericFormattableString>()
            .Add(NamedResults.Name, _evaluator.EvaluateInterpolatedStringAsync(context.Request.Settings.BuilderExtensionsNameFormatString, context.Request.FormatProvider, context.Request, token))
            .Add(NamedResults.Namespace, _evaluator.EvaluateInterpolatedStringAsync(context.Request.Settings.BuilderExtensionsNamespaceFormatString, context.Request.FormatProvider, context.Request, token))
            .Build()
            .ConfigureAwait(false);

        var error = results.GetError();
        if (error is not null)
        {
            // Error in formattable string parsing
            return error;
        }

        context.Request.Builder
            .WithName(results[NamedResults.Name].Value!)
            .WithNamespace(results[NamedResults.Namespace].Value!);

        return Result.Success();
    }
}

namespace ClassFramework.Pipelines.Interface.Components;

public class SetNameComponent(IExpressionEvaluator evaluator) : IPipelineComponent<InterfaceContext>
{
    private readonly IExpressionEvaluator _evaluator = evaluator.IsNotNull(nameof(evaluator));

    public async Task<Result> ProcessAsync(PipelineContext<InterfaceContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        var results = await new AsyncResultDictionaryBuilder<GenericFormattableString>()
            .Add(NamedResults.Name, _evaluator.EvaluateAsync(context.Request.Settings.NameFormatString, context.Request.FormatProvider, context.Request, token))
            .Add(NamedResults.Namespace, context.Request.GetMappingMetadata(context.Request.SourceModel.GetFullName()).GetGenericFormattableStringAsync(MetadataNames.CustomEntityNamespace, _evaluator.EvaluateAsync(context.Request.Settings.NamespaceFormatString, context.Request.FormatProvider, context.Request, token)))
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
            .WithNamespace(context.Request.MapNamespace(results[NamedResults.Namespace].Value!));

        return Result.Success();
    }
}

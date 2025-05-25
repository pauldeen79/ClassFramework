namespace ClassFramework.Pipelines.Builder.Components;

public class SetNameComponent(IExpressionEvaluator evaluator) : IPipelineComponent<BuilderContext>
{
    private readonly IExpressionEvaluator _evaluator = evaluator.IsNotNull(nameof(evaluator));

    public async Task<Result> ProcessAsync(PipelineContext<BuilderContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        return (await new AsyncResultDictionaryBuilder<GenericFormattableString>()
            .Add(NamedResults.Name, _evaluator.Parse(context.Request.Settings.BuilderNameFormatString, context.Request.FormatProvider, context.Request, token))
            .Add(NamedResults.Namespace, context.Request.GetMappingMetadata(context.Request.SourceModel.GetFullName()).GetGenericFormattableString(MetadataNames.CustomBuilderNamespace, _evaluator.Parse(context.Request.Settings.BuilderNamespaceFormatString, context.Request.FormatProvider, context.Request, token)))
            .Build()
            .ConfigureAwait(false))
            .OnSuccess(results =>
            {
                context.Request.Builder
                    .WithName(results.GetValue(NamedResults.Name))
                    .WithNamespace(results.GetValue(NamedResults.Namespace));
            });
    }
}

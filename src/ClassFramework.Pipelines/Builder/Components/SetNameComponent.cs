namespace ClassFramework.Pipelines.Builder.Components;

public class SetNameComponent(IExpressionEvaluator evaluator) : IPipelineComponent<BuilderContext>
{
    private readonly IExpressionEvaluator _evaluator = evaluator.IsNotNull(nameof(evaluator));

    public Task<Result> ProcessAsync(PipelineContext<BuilderContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        return Task.FromResult(new ResultDictionaryBuilder<GenericFormattableString>()
            .Add(NamedResults.Name, () => _evaluator.Parse(context.Request.Settings.BuilderNameFormatString, context.Request.FormatProvider, context.Request))
            .Add(NamedResults.Namespace, () => context.Request.GetMappingMetadata(context.Request.SourceModel.GetFullName()).GetGenericFormattableString(MetadataNames.CustomBuilderNamespace, () => _evaluator.Parse(context.Request.Settings.BuilderNamespaceFormatString, context.Request.FormatProvider, context.Request)))
            .Build()
            .OnSuccess(results =>
            {
                context.Request.Builder
                    .WithName(results[NamedResults.Name].Value!)
                    .WithNamespace(results[NamedResults.Namespace].Value!);
            }));
    }
}

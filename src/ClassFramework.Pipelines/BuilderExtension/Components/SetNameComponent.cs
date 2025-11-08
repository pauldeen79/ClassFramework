namespace ClassFramework.Pipelines.BuilderExtension.Components;

public class SetNameComponent(IExpressionEvaluator evaluator) : IPipelineComponent<BuilderExtensionContext, ClassBuilder>
{
    private readonly IExpressionEvaluator _evaluator = evaluator.IsNotNull(nameof(evaluator));

    public async Task<Result> ExecuteAsync(BuilderExtensionContext context, ClassBuilder response, ICommandService commandService, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));
        response = response.IsNotNull(nameof(response));

        return (await new AsyncResultDictionaryBuilder<GenericFormattableString>()
            .Add(ResultNames.Name, () => _evaluator.EvaluateInterpolatedStringAsync(context.Settings.BuilderExtensionsNameFormatString, context.FormatProvider, context, token))
            .Add(ResultNames.Namespace, () => _evaluator.EvaluateInterpolatedStringAsync(context.Settings.BuilderExtensionsNamespaceFormatString, context.FormatProvider, context, token))
            .Build()
            .ConfigureAwait(false))
            .OnSuccess(results =>
            {
                response
                    .WithName(results.GetValue(ResultNames.Name))
                    .WithNamespace(results.GetValue(ResultNames.Namespace));
            });
    }
}

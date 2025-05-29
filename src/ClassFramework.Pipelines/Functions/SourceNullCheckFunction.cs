namespace ClassFramework.Pipelines.Functions;

public class SourceNullCheckFunction : IFunction<string>
{
    public async Task<Result<object?>> EvaluateAsync(FunctionCallContext context, CancellationToken token)
        => await EvaluateTypedAsync(context, token).ConfigureAwait(false);

    public async Task<Result<string>> EvaluateTypedAsync(FunctionCallContext context, CancellationToken token)
    {
        context = ArgumentGuard.IsNotNull(context, nameof(context));

        return (await new AsyncResultDictionaryBuilder()
            .Add("settings", context.GetSettingsAsync())
            .Add("context", context.Context.State.TryGetValueAsync<ContextBase>("context"))
            .Build()
            .ConfigureAwait(false))
            .OnSuccess(results => results.GetValue<PipelineSettings>("settings").AddNullChecks
                ? results.GetValue<ContextBase>("context").CreateArgumentNullException("source")
                : string.Empty);
    }
}

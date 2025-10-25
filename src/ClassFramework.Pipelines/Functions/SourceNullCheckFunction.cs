namespace ClassFramework.Pipelines.Functions;

public class SourceNullCheckFunction : IFunction<string>
{
    public async Task<Result<object?>> EvaluateAsync(FunctionCallContext context, CancellationToken token)
        => await EvaluateTypedAsync(context, token).ConfigureAwait(false);

    public async Task<Result<string>> EvaluateTypedAsync(FunctionCallContext context, CancellationToken token)
    {
        context = ArgumentGuard.IsNotNull(context, nameof(context));

        return (await new AsyncResultDictionaryBuilder()
            .Add(ResultNames.Settings, () => context.GetSettingsAsync())
            .Add(ResultNames.Context, () => context.Context.State.TryCastValueAsync<ContextBase>(ResultNames.Context))
            .BuildAsync()
            .ConfigureAwait(false))
            .OnSuccess(results => results.GetValue<PipelineSettings>(ResultNames.Settings).AddNullChecks
                ? results.GetValue<ContextBase>(ResultNames.Context).CreateArgumentNullException("source")
                : string.Empty);
    }
}

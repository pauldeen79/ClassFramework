namespace ClassFramework.Pipelines.Functions;

public class InstancePrefixFunction : IFunction<string>
{
    public async Task<Result<object?>> EvaluateAsync(FunctionCallContext context, CancellationToken token)
        => await EvaluateTypedAsync(context, token).ConfigureAwait(false);

    public async Task<Result<string>> EvaluateTypedAsync(FunctionCallContext context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        var contextValue = (await context.Context.State[ResultNames.Context].ConfigureAwait(false)).Value;
        var value = contextValue is BuilderExtensionContext
            ? "instance."
            : string.Empty;

        return Result.Success(value);
    }
}

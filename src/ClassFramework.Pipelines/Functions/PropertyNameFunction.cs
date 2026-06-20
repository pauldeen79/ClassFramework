namespace ClassFramework.Pipelines.Functions;

public class PropertyNameFunction : IFunction<string>
{
    public async Task<Result<object?>> EvaluateAsync(FunctionCallContext context, CancellationToken token)
        => await EvaluateTypedAsync(context, token).ConfigureAwait(false);

    public Task<Result<string>> EvaluateTypedAsync(FunctionCallContext context, CancellationToken token)
    {
        context = ArgumentGuard.IsNotNull(context, nameof(context));

        return FunctionHelpers.ParseFromContextAsync(context, "NullCheck", c => Result.From(!c.Settings.AddProperties
            ? "_{property.Name.ToCamelCase()}"
            : "{property.Name}"));
    }
}

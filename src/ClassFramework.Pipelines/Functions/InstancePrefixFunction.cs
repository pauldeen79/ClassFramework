namespace ClassFramework.Pipelines.Functions;

public class InstancePrefixFunction : IFunction
{
    public async Task<Result<object?>> EvaluateAsync(FunctionCallContext context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        var contextValue = (await context.Context.State["context"].ConfigureAwait(false)).Value;
        var value = contextValue is ParentChildContext<PipelineContext<BuilderExtensionContext>, Property>
            ? "instance."
            : string.Empty;

        return Result.Success<object?>(value);
    }
}

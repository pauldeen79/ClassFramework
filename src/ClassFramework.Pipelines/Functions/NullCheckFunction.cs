namespace ClassFramework.Pipelines.Functions;

public class NullCheckFunction : IFunction
{
    public Task<Result<object?>> EvaluateAsync(FunctionCallContext context, CancellationToken token)
        => FunctionHelpers.ParseFromContext(context.IsNotNull(nameof(context)), "NullCheck", c => Result.Success<object?>(c.NullCheck));
}

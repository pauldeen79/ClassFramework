namespace ClassFramework.Pipelines.Functions;

public class NoGenericsFunction : IFunction
{
    public Task<Result<object?>> EvaluateAsync(FunctionCallContext context, CancellationToken token)
        => FunctionHelpers.ParseFromStringArgument(context, "NoGenerics", s => Result.Success<object?>(s.WithoutGenerics()), token);
}

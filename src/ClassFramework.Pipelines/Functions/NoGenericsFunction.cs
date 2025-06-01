namespace ClassFramework.Pipelines.Functions;

[MemberArgument(Constants.Expression, typeof(string))]
public class NoGenericsFunction : IFunction
{
    public Task<Result<object?>> EvaluateAsync(FunctionCallContext context, CancellationToken token)
        => FunctionHelpers.ParseFromStringArgumentAsync(context, "NoGenerics", s => Result.Success<object?>(s.WithoutGenerics()), token);
}

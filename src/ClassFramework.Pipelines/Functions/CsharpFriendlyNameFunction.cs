namespace ClassFramework.Pipelines.Functions;

public class CsharpFriendlyNameFunction : IFunction
{
    public Task<Result<object?>> EvaluateAsync(FunctionCallContext context, CancellationToken token)
        => FunctionHelpers.ParseFromStringArgumentAsync(context, "CsharpFriendlyName", s => Result.Success<object?>(s.GetCsharpFriendlyName()), token);
}

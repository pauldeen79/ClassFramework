namespace ClassFramework.Pipelines.Functions;

public class CsharpFriendlyTypeNameFunction : IFunction
{
    public Task<Result<object?>> EvaluateAsync(FunctionCallContext context, CancellationToken token)
        => FunctionHelpers.ParseFromStringArgumentAsync(context, "CsharpFriendlyTypeName", s => Result.Success<object?>(s.GetCsharpFriendlyTypeName()), token);
}

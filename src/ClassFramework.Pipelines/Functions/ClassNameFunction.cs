namespace ClassFramework.Pipelines.Functions;

public class ClassNameFunction : IFunction
{
    public Task<Result<object?>> EvaluateAsync(FunctionCallContext context, CancellationToken token)
        => FunctionHelpers.ParseFromStringArgumentAsync(context, "ClassName", s => Result.Success<object?>(s.GetClassName()), token);
}

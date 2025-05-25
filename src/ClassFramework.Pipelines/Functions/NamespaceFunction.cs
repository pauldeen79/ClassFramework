namespace ClassFramework.Pipelines.Functions;

public class NamespaceFunction : IFunction
{
    public Task<Result<object?>> EvaluateAsync(FunctionCallContext context, CancellationToken token)
        => FunctionHelpers.ParseFromStringArgument(context, "Namespace", s => Result.Success<object?>(s.GetNamespaceWithDefault()));
}

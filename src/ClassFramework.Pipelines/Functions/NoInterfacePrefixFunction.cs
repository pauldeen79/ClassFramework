namespace ClassFramework.Pipelines.Functions;

public class NoInterfacePrefixFunction : IFunction
{
    public Task<Result<object?>> EvaluateAsync(FunctionCallContext context, CancellationToken token)
        => FunctionHelpers.ParseFromStringArgument(context, "NoInterfacePrefix", s => Result.Success<object?>(s.WithoutInterfacePrefix()), token);
}

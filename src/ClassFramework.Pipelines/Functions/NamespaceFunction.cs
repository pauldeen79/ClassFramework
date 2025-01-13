namespace ClassFramework.Pipelines.Functions;

public class NamespaceFunction : IFunction
{
    public Result<object?> Evaluate(FunctionCallContext context)
        => FunctionBase.ParseFromStringArgument(context, "Namespace", s => Result.Success<object?>(s.GetNamespaceWithDefault()));

    public Result Validate(FunctionCallContext context)
        => Result.Success();
}

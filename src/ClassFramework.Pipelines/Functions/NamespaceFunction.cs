namespace ClassFramework.Pipelines.Functions;

public class NamespaceFunction : IFunction
{
    public Result<object?> Evaluate(FunctionCallContext context)
        => FunctionHelpers.ParseFromStringArgument(context, "Namespace", s => Result.Success<object?>(s.GetNamespaceWithDefault()));
}

namespace ClassFramework.Pipelines.Functions;

public class NullCheckFunction : IFunction
{
    public Result<object?> Evaluate(FunctionCallContext context)
        => FunctionBase.ParseFromContext(context.IsNotNull(nameof(context)), "NullCheck", c => Result.Success<object?>(c.NullCheck));
}

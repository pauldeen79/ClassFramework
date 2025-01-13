namespace ClassFramework.Pipelines.Functions;

public class NoGenericsFunction : IFunction
{
    public Result<object?> Evaluate(FunctionCallContext context)
        => FunctionBase.ParseFromStringArgument(context, "NoGenerics", s => Result.Success<object?>(s.WithoutGenerics()));

    public Result Validate(FunctionCallContext context)
        => Result.Success();
}

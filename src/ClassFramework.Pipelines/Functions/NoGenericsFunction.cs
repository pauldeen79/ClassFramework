namespace ClassFramework.Pipelines.Functions;

public class NoGenericsFunction : IFunction
{
    public Result<object?> Evaluate(FunctionCallContext context)
        => FunctionHelpers.ParseFromStringArgument(context, "NoGenerics", s => Result.Success<object?>(s.WithoutGenerics()));
}

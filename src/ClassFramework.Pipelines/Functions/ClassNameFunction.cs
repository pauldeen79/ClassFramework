namespace ClassFramework.Pipelines.Functions;

public class ClassNameFunction : IFunction
{
    public Result<object?> Evaluate(FunctionCallContext context)
        => FunctionHelpers.ParseFromStringArgument(context, "ClassName", s => Result.Success<object?>(s.GetClassName()));
}

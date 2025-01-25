namespace ClassFramework.Pipelines.Functions;

public class CsharpFriendlyNameFunction : IFunction
{
    public Result<object?> Evaluate(FunctionCallContext context)
        => FunctionHelpers.ParseFromStringArgument(context, "CsharpFriendlyName", s => Result.Success<object?>(s.GetCsharpFriendlyName()));
}

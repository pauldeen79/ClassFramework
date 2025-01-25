namespace ClassFramework.Pipelines.Functions;

public class CsharpFriendlyNameFunction : IFunction
{
    public Result<object?> Evaluate(FunctionCallContext context)
        => FunctionBase.ParseFromStringArgument(context, "CsharpFriendlyName", s => Result.Success<object?>(s.GetCsharpFriendlyName()));
}

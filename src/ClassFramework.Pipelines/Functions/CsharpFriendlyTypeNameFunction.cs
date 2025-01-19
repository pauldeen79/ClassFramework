namespace ClassFramework.Pipelines.Functions;

public class CsharpFriendlyTypeNameFunction : IFunction
{
    public Result<object?> Evaluate(FunctionCallContext context)
        => FunctionBase.ParseFromStringArgument(context, "CsharpFriendlyTypeName", s => Result.Success<object?>(s.GetCsharpFriendlyTypeName()));

    public Result Validate(FunctionCallContext context)
        => Result.Success();
}

namespace ClassFramework.Pipelines.Functions;

public class CsharpFriendlyTypeNameFunction : IFunction
{
    public Result<object?> Evaluate(FunctionCallContext context)
        => FunctionHelpers.ParseFromStringArgument(context, "CsharpFriendlyTypeName", s => Result.Success<object?>(s.GetCsharpFriendlyTypeName()));
}

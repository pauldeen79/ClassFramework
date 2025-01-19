namespace ClassFramework.Pipelines.Functions;

public class NoInterfacePrefixFunction : IFunction
{
    public Result<object?> Evaluate(FunctionCallContext context)
        => FunctionBase.ParseFromStringArgument(context, "NoInterfacePrefix", s => Result.Success<object?>(s.WithoutInterfacePrefix()));

    public Result Validate(FunctionCallContext context)
        => Result.Success();
}

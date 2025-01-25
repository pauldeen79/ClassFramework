namespace ClassFramework.Pipelines.Functions;

public class NoInterfacePrefixFunction : IFunction
{
    public Result<object?> Evaluate(FunctionCallContext context)
        => FunctionBase.ParseFromStringArgument(context, "NoInterfacePrefix", s => Result.Success<object?>(s.WithoutInterfacePrefix()));
}

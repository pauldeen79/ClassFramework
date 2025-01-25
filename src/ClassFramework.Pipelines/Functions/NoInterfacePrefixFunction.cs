namespace ClassFramework.Pipelines.Functions;

public class NoInterfacePrefixFunction : IFunction
{
    public Result<object?> Evaluate(FunctionCallContext context)
        => FunctionHelpers.ParseFromStringArgument(context, "NoInterfacePrefix", s => Result.Success<object?>(s.WithoutInterfacePrefix()));
}

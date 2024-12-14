namespace ClassFramework.Pipelines.Functions;

public class NoInterfacePrefixFunction : IFunctionResultParser
{
    public Result<object?> Parse(FunctionParseResult functionParseResult, object? context, IFunctionParseResultEvaluator evaluator, IExpressionParser parser)
        => FunctionBase.ParseFromStringArgument(functionParseResult, context, evaluator, parser, "NoInterfacePrefix", s => Result.Success<object?>(s.WithoutInterfacePrefix()));
}

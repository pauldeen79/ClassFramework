namespace ClassFramework.Pipelines.Functions;

public class NullCheckFunction : IFunction
{
    public Result<object?> Parse(FunctionParseResult functionParseResult, object? context, IFunctionParseResultEvaluator evaluator, IExpressionParser parser)
        => FunctionBase.ParseFromContext(functionParseResult, context, "NullCheck", c => Result.Success<object?>(c.NullCheck));
}

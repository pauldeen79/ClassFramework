namespace ClassFramework.Pipelines.Functions;

public class NullCheckFunction : IFunctionResultParser
{
    public Result<object?> Parse(FunctionParseResult functionParseResult, object? context, IFunctionParseResultEvaluator evaluator, IExpressionParser parser)
    {
        functionParseResult = functionParseResult.IsNotNull(nameof(functionParseResult));

        return FunctionBase.ParseFromContext(functionParseResult, context, evaluator, parser, "NullCheck", c => Result.Success<object?>(c.NullCheck));
    }
}

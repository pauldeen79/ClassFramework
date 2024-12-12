namespace ClassFramework.Pipelines.Shared.Functions;

public class CsharpFriendlyNameFunction : IFunctionResultParser
{
    public Result<object?> Parse(FunctionParseResult functionParseResult, object? context, IFunctionParseResultEvaluator evaluator, IExpressionParser parser)
    {
        functionParseResult = functionParseResult.IsNotNull(nameof(functionParseResult));

        return FunctionBase.Parse(functionParseResult, context, evaluator, parser, "CsharpFriendlyName", s => Result.Success<object?>(s.GetCsharpFriendlyName()));
    }
}

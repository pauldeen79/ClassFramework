namespace ClassFramework.Pipelines.Functions;

public class CsharpFriendlyNameFunction : IFunction
{
    public Result<object?> Parse(FunctionParseResult functionParseResult, object? context, IFunctionParseResultEvaluator evaluator, IExpressionParser parser)
        => FunctionBase.ParseFromStringArgument(functionParseResult, context, evaluator, parser, "CsharpFriendlyName", s => Result.Success<object?>(s.GetCsharpFriendlyName()));
}

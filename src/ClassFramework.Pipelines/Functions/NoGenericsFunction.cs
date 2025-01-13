namespace ClassFramework.Pipelines.Functions;

public class NoGenericsFunction : IFunction
{
    public Result<object?> Parse(FunctionParseResult functionParseResult, object? context, IFunctionParseResultEvaluator evaluator, IExpressionParser parser)
        => FunctionBase.ParseFromStringArgument(functionParseResult, context, evaluator, parser, "NoGenerics", s => Result.Success<object?>(s.WithoutGenerics()));
}

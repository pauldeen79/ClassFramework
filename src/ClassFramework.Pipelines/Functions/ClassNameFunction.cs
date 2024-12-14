namespace ClassFramework.Pipelines.Functions;

public class ClassNameFunction : IFunctionResultParser
{
    public Result<object?> Parse(FunctionParseResult functionParseResult, object? context, IFunctionParseResultEvaluator evaluator, IExpressionParser parser)
        => FunctionBase.ParseFromStringArgument(functionParseResult, context, evaluator, parser, "ClassName", s => Result.Success<object?>(s.GetClassName()));
}

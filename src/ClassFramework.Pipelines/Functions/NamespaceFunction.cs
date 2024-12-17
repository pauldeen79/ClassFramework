namespace ClassFramework.Pipelines.Functions;

public class NamespaceFunction : IFunctionResultParser
{
    public Result<object?> Parse(FunctionParseResult functionParseResult, object? context, IFunctionParseResultEvaluator evaluator, IExpressionParser parser)
        => FunctionBase.ParseFromStringArgument(functionParseResult, context, evaluator, parser, "Namespace", s => Result.Success<object?>(s.GetNamespaceWithDefault()));
}

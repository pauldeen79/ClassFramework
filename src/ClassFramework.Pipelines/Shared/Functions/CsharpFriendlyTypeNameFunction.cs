namespace ClassFramework.Pipelines.Shared.Functions;

public class CsharpFriendlyTypeNameFunction : IFunctionResultParser
{
    public Result<object?> Parse(FunctionParseResult functionParseResult, object? context, IFunctionParseResultEvaluator evaluator, IExpressionParser parser)
    {
        functionParseResult = functionParseResult.IsNotNull(nameof(functionParseResult));

        return FunctionBase.ParseFromStringArgument(functionParseResult, context, evaluator, parser, "CsharpFriendlyTypeName", s => Result.Success<object?>(s.GetCsharpFriendlyTypeName()));
    }
}

namespace ClassFramework.Pipelines.Shared.Functions;

internal static class FunctionBase
{
    internal static Result<object?> Parse(FunctionParseResult functionParseResult, object? context, IFunctionParseResultEvaluator evaluator, IExpressionParser parser, string functionName, Func<string, string> functionDelegate)
    {
        if (functionParseResult.FunctionName != functionName)
        {
            return Result.Continue<object?>();
        }

        var argument = functionParseResult.Arguments.FirstOrDefault();
        if (argument is null)
        {
            return Result.Invalid<object?>($"{functionName} function requires one argument");
        }

        var result = argument.GetValueResult(context, evaluator, parser, functionParseResult.FormatProvider);
        if (!result.IsSuccessful())
        {
            return result;
        }

        if (result.Value is not string s)
        {
            return Result.Invalid<object?>($"{functionName} does not support type {result.Value?.GetType().FullName}, only string is supported");
        }

        return Result.Success<object?>(functionDelegate(s));
    }
}

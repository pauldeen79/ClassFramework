namespace ClassFramework.Pipelines.Functions;

internal static class FunctionBase
{
    internal static Result<object?> ParseFromStringArgument(FunctionParseResult functionParseResult, object? context, IFunctionParseResultEvaluator evaluator, IExpressionParser parser, string functionName, Func<string, Result<object?>> functionDelegate)
    {
        functionParseResult = functionParseResult.IsNotNull(nameof(functionParseResult));
        evaluator = evaluator.IsNotNull(nameof(evaluator));
        parser = parser.IsNotNull(nameof(parser));

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

        if (result.Value is null)
        {
            return Result.Invalid<object?>($"{functionName} function requires argument of type string, but the value was null");
        }

        if (result.Value is not string s)
        {
            return Result.Invalid<object?>($"{functionName} function does not support type {result.Value.GetType().FullName}, only string is supported");
        }

        return functionDelegate(s);
    }

    internal static Result<object?> ParseFromContext(FunctionParseResult functionParseResult, object? context, string functionName, Func<ContextBase, Result<object?>> functionDelegate)
    {
        functionParseResult = functionParseResult.IsNotNull(nameof(functionParseResult));
        
        if (functionParseResult.FunctionName != functionName)
        {
            return Result.Continue<object?>();
        }

        return context switch
        {
            ContextBase contextBase => functionDelegate(contextBase),
            ParentChildContext<PipelineContext<EntityContext>, Property> parentChildContextEntity => functionDelegate(parentChildContextEntity.ParentContext.Request),
            _ => Result.Invalid<object?>($"{functionName} function does not support type {context?.GetType().FullName ?? "null"}, only ContextBase is supported")
        };
    }
}

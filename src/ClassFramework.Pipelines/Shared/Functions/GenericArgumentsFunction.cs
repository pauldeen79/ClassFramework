namespace ClassFramework.Pipelines.Shared.Functions;

public class GenericArgumentsFunction : IFunctionResultParser
{
    public Result<object?> Parse(FunctionParseResult functionParseResult, object? context, IFunctionParseResultEvaluator evaluator, IExpressionParser parser)
    {
        functionParseResult = functionParseResult.IsNotNull(nameof(functionParseResult));

        return FunctionBase.Parse(functionParseResult, context, evaluator, parser, "GenericArguments", s =>
        {
            var addBrackets = false;
            if (functionParseResult.Arguments.Count >= 2)
            {
                var result = functionParseResult.Arguments.ElementAt(1).GetValueResult(context, evaluator, parser, functionParseResult.FormatProvider);
                if (!result.IsSuccessful())
                {
                    return result;
                }

                if (result.Value is not bool addBracketsValue)
                {
                    return Result.Invalid<object?>("GenericArguments function second argument (add brackets) should be boolean");
                }

                addBrackets = addBracketsValue;
            }

            return Result.Success<object?>(s.GetProcessedGenericArguments(addBrackets));
        });
    }
}

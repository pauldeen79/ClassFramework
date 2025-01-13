namespace ClassFramework.Pipelines.Functions;

public class GenericArgumentsFunction : IFunction
{
    public Result<object?> Evaluate(FunctionCallContext context)
        => FunctionBase.ParseFromStringArgument(context, "GenericArguments", s =>
        {
            var addBrackets = false;
            if (context.FunctionCall.Arguments.Count >= 2)
            {
                var result = context.FunctionCall.Arguments.ElementAt(1).GetValueResult(context);
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

    public Result Validate(FunctionCallContext context)
        => Result.Success();
}

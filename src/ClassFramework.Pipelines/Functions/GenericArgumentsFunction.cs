namespace ClassFramework.Pipelines.Functions;

[MemberArgument("Expression", typeof(string))]
[MemberArgument("AddBrackets", typeof(bool), false)]
public class GenericArgumentsFunction : IFunction
{
    public async Task<Result<object?>> EvaluateAsync(FunctionCallContext context, CancellationToken token)
    {
        context = ArgumentGuard.IsNotNull(context, nameof(context));

        var expressionResult = await context.GetArgumentValueResultAsync<string>(0, "Expression", token).ConfigureAwait(false);
        if (!expressionResult.IsSuccessful())
        {
            return expressionResult;
        }

        var addBrackets = false;
        if (context.FunctionCall.Arguments.Count >= 2)
        {
            var result = await context.FunctionCall.GetArgumentValueResultAsync(1, "AddBrackets", context, token).ConfigureAwait(false);
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

        return Result.Success<object?>(expressionResult.Value.GetGenericArguments(addBrackets));
    }
}

namespace ClassFramework.Pipelines.Functions;

public class ToCamelCaseFunction : IFunction
{
    public Result<object?> Evaluate(FunctionCallContext context)
    {
        context = ArgumentGuard.IsNotNull(context, nameof(context));

        return new ResultDictionaryBuilder()
            .Add("Expression", () => context.GetArgumentStringValueResult(0, "Expression"))
            .Build()
            .OnSuccess(results => Result.Success<object?>(results.GetValue<string>("Expression").ToCamelCase(context.FormatProvider.ToCultureInfo())));
    }
}

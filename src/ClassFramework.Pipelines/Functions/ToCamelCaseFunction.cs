namespace ClassFramework.Pipelines.Functions;

public class ToCamelCaseFunction : IFunction
{
    public Result<object?> Evaluate(FunctionCallContext context)
        => FunctionHelpers.ParseFromStringArgument(context, "GenericArguments", s => Result.Success<object?>(s.ToCamelCase(context.FormatProvider.ToCultureInfo())));
}

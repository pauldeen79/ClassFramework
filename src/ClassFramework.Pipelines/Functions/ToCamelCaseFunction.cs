namespace ClassFramework.Pipelines.Functions;

public class ToCamelCaseFunction : IFunction
{
    public Result<object?> Evaluate(FunctionCallContext context)
        => FunctionHelpers.ParseFromStringArgument(context, "ToCamelCase", s => Result.Success<object?>(s.ToCamelCase(context.Settings.FormatProvider.ToCultureInfo())));
}

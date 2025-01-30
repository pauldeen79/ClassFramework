namespace ClassFramework.Pipelines.Functions;

public class ToPascalCaseFunction : IFunction
{
    public Result<object?> Evaluate(FunctionCallContext context)
        => FunctionHelpers.ParseFromStringArgument(context, "ToPascalCase", s => Result.Success<object?>(s.ToPascalCase(context.Settings.FormatProvider.ToCultureInfo())));
}

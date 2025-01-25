namespace ClassFramework.Pipelines.Functions;

public class ToPascalCaseFunction : IFunction
{
    public Result<object?> Evaluate(FunctionCallContext context)
        => FunctionHelpers.ParseFromStringArgument(context, "GenericArguments", s => Result.Success<object?>(s.ToPascalCase(context.FormatProvider.ToCultureInfo())));
}

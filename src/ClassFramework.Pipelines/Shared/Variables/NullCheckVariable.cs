namespace ClassFramework.Pipelines.Shared.Variables;

public class NullCheckVariable : IVariable
{
    public Result<object?> Process(string variableExpression, object? context)
    {
        if (variableExpression == "nullCheck")
        {
            var value = context is ContextBase contextBase
                ? contextBase.NullCheck
                : string.Empty;

            return Result.Success<object?>(value);
        }

        return Result.Continue<object?>();
    }
}

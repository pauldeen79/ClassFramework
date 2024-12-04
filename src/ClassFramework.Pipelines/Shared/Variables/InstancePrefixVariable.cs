namespace ClassFramework.Pipelines.Shared.Variables;

public class InstancePrefixVariable : IVariable
{
    public Result<object?> Process(string variableExpression, object? context)
    {
        if (variableExpression == "instancePrefix")
        {
            var value = context is ParentChildContext<PipelineContext<BuilderExtensionContext>, Property>
                ? "instance."
                : string.Empty;

            return Result.Success<object?>(value);
        }

        return Result.Continue<object?>();
    }
}

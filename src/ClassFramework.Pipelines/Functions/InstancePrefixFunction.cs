namespace ClassFramework.Pipelines.Functions;

public class InstancePrefixFunction : IFunction
{
    public Result<object?> Evaluate(FunctionCallContext context)
    {
        context = context.IsNotNull(nameof(context));

        var value = context.Context is ParentChildContext<PipelineContext<BuilderExtensionContext>, Property>
            ? "instance."
            : string.Empty;

        return Result.Success<object?>(value);
    }

    public Result Validate(FunctionCallContext context)
        => Result.Success();
}

namespace ClassFramework.Pipelines.Shared.Functions;

public class InstancePrefixFunction : IFunctionResultParser
{
    public Result<object?> Parse(FunctionParseResult functionParseResult, object? context, IFunctionParseResultEvaluator evaluator, IExpressionParser parser)
    {
        functionParseResult = functionParseResult.IsNotNull(nameof(functionParseResult));

        if (functionParseResult.FunctionName != "InstancePrefix")
        {
            return Result.Continue<object?>();
        }

        var value = context is ParentChildContext<PipelineContext<BuilderExtensionContext>, Property>
            ? "instance."
            : string.Empty;

        return Result.Success<object?>(value);
    }
}

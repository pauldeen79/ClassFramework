namespace ClassFramework.Pipelines.Functions;

internal static class FunctionHelpers
{
    internal static Result<object?> ParseFromStringArgument(FunctionCallContext context, string functionName, Func<string, Result<object?>> functionDelegate)
        => new ResultDictionaryBuilder()
            .Add("Expression", () => context.IsNotNull(nameof(context)).GetArgumentStringValueResult(0, "Expression"))
            .Build()
            .OnSuccess(results => functionDelegate(results.GetValue<string>("Expression")));

    internal static Result<object?> ParseFromContext(FunctionCallContext context, string functionName, Func<ContextBase, Result<object?>> functionDelegate)
        => context.IsNotNull(nameof(context)).Context switch
        {
            ContextBase contextBase => functionDelegate(contextBase),
            ParentChildContext<PipelineContext<EntityContext>, Property> parentChildContextEntity => functionDelegate(parentChildContextEntity.ParentContext.Request),
            _ => Result.Invalid<object?>($"{functionName} function does not support type {context!.Context?.GetType().FullName ?? "null"}, only ContextBase is supported")
        };
}

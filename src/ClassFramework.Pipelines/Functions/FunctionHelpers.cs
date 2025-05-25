namespace ClassFramework.Pipelines.Functions;

internal static class FunctionHelpers
{
    internal static async Task<Result<object?>> ParseFromStringArgument(FunctionCallContext context, string functionName, Func<string, Result<object?>> functionDelegate, CancellationToken token)
        => (await new AsyncResultDictionaryBuilder()
            .Add("Expression", context.IsNotNull(nameof(context)).GetArgumentValueResultAsync(0, "Expression", token))
            .Build()
            .ConfigureAwait(false))
            .OnSuccess(results => functionDelegate(results.GetValue<string>("Expression")));

    internal static async Task<Result<object?>> ParseFromContext(FunctionCallContext context, string functionName, Func<ContextBase, Result<object?>> functionDelegate)
        //TODO: Replace ["context"]
        => (await context.IsNotNull(nameof(context)).Context.State["context"].ConfigureAwait(false)).Value switch
        {
            ContextBase contextBase => functionDelegate(contextBase),
            ParentChildContext<PipelineContext<EntityContext>, Property> parentChildContextEntity => functionDelegate(parentChildContextEntity.ParentContext.Request),
            _ => Result.Invalid<object?>($"{functionName} function does not support type {context!.Context?.GetType().FullName ?? "null"}, only ContextBase is supported")
        };
}

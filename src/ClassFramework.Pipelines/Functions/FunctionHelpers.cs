namespace ClassFramework.Pipelines.Functions;

internal static class FunctionHelpers
{
    internal static async Task<Result<object?>> ParseFromStringArgumentAsync(FunctionCallContext context, string functionName, Func<string, Result<object?>> functionDelegate, CancellationToken token)
        => (await new AsyncResultDictionaryBuilder()
            .Add("Expression", (await context.IsNotNull(nameof(context)).GetArgumentValueResultAsync(0, "Expression", token).ConfigureAwait(false)).TryCast<string>())
            .Build()
            .ConfigureAwait(false))
            .OnSuccess(results => functionDelegate(results.GetValue<string>("Expression")));

    internal static async Task<Result<object?>> ParseFromContextAsync(FunctionCallContext context, string functionName, Func<ContextBase, Result<object?>> functionDelegate)
        => (await context.IsNotNull(nameof(context)).Context.State["context"].ConfigureAwait(false)).Value switch
        {
            ContextBase contextBase => functionDelegate(contextBase),
            ParentChildContext<PipelineContext<EntityContext>, Property> parentChildContextEntity => functionDelegate(parentChildContextEntity.ParentContext.Request),
            _ => Result.Invalid<object?>($"{functionName} function does not support type {context!.Context?.GetType().FullName ?? "null"}, only ContextBase is supported")
        };
}

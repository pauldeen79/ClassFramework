namespace ClassFramework.Pipelines.Functions;

internal static class FunctionHelpers
{
    internal static async Task<Result<object?>> ParseFromStringArgumentAsync(FunctionCallContext context, string functionName, Func<string, Result<object?>> functionDelegate, CancellationToken token)
        => (await new AsyncResultDictionaryBuilder()
            .Add(Constants.Expression, (await context.IsNotNull(nameof(context)).GetArgumentValueResultAsync(0, Constants.Expression, token).ConfigureAwait(false)).TryCast<string>())
            .Build()
            .ConfigureAwait(false))
            .OnSuccess(results => functionDelegate(results.GetValue<string>(Constants.Expression)));

    internal static async Task<Result<object?>> ParseFromContextAsync(FunctionCallContext context, string functionName, Func<ContextBase, Result<object?>> functionDelegate)
    {
        var ctx = await context.IsNotNull(nameof(context)).Context.State[ResultNames.Context].ConfigureAwait(false);

        return ctx.Value switch
        {
            ContextBase contextBase => functionDelegate(contextBase),
            ParentChildContext<PipelineContext<EntityContext>, Property> parentChildContextEntity => functionDelegate(parentChildContextEntity.ParentContext.Request),
            _ => Result.Invalid<object?>($"{functionName} function does not support type {ctx.Value?.GetType().FullName ?? "null"}, only ContextBase is supported")
        };
    }
}

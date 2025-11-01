namespace ClassFramework.Pipelines.Functions;

internal static class FunctionHelpers
{
    internal static async Task<Result<T>> ParseFromStringArgumentAsync<T>(FunctionCallContext context, string functionName, Func<string, Result<T>> functionDelegate, CancellationToken token)
        => (await new AsyncResultDictionaryBuilder()
            .Add(Constants.Expression, async () => (await context.GetArgumentValueResultAsync(0, Constants.Expression, token).ConfigureAwait(false)).TryCast<string>())
            .BuildAsync()
            .ConfigureAwait(false))
            .OnFailure(result => result.Wrap($"{functionName} function failed, see inner results for details"))
            .OnSuccess(results => functionDelegate(results.GetValue<string>(Constants.Expression)));

    internal static async Task<Result<string>> ParseFromContextAsync(FunctionCallContext context, Func<ContextBase, PipelineSettings, ClassModel, Property, bool, string> resultDelegate)
        => (await new AsyncResultDictionaryBuilder()
            .Add(ResultNames.Settings, context.GetSettingsAsync)
            .Add(ResultNames.Context, () => context.Context.State.TryCastValueAsync<ContextBase>(ResultNames.Context))
            .Add(ResultNames.Property, () => context.Context.State.TryCastValueAsync<Property>(ResultNames.Property))
            .Add(ResultNames.Class, () => context.Context.State.TryCastValueAsync<ClassModel>(ResultNames.Class))
            .BuildAsync()
            .ConfigureAwait(false))
            .OnSuccess(results =>
            {
                var contextBase = results.GetValue<ContextBase>(ResultNames.Context);
                var settings = results.GetValue<PipelineSettings>(ResultNames.Settings);
                var classModel = results.GetValue<ClassModel>(ResultNames.Class);
                var property = results.GetValue<Property>(ResultNames.Property);

                // note that for now, we assume that a generic type argument should not be included in argument null checks...
                // this might be the case (for example there is a constraint on class), but this is not supported yet
                var isGenericArgument = classModel.GetGenericTypeArguments().Contains(property.TypeName);

                return resultDelegate(contextBase, settings, classModel, property, isGenericArgument);
            });

    internal static async Task<Result<T>> ParseFromContextAsync<T>(FunctionCallContext context, string functionName, Func<ContextBase, Result<T>> functionDelegate)
    {
        var ctx = await context.Context.State[ResultNames.Context]().ConfigureAwait(false);

        return ctx.Value switch
        {
            ContextBase contextBase => functionDelegate(contextBase),
            ParentChildContext<EntityContext, Property> parentChildContextEntity => functionDelegate(parentChildContextEntity.ParentContext),
            _ => Result.Invalid<T>($"{functionName} function does not support type {ctx.Value?.GetType().FullName ?? "null"}, only ContextBase is supported")
        };
    }
}

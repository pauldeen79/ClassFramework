namespace ClassFramework.Pipelines.Functions;

[MemberArgument("setter", typeof(bool), false)]
public class PropertyNameFunction : IFunction<string>
{
    public async Task<Result<object?>> EvaluateAsync(FunctionCallContext context, CancellationToken token)
        => await EvaluateTypedAsync(context, token).ConfigureAwait(false);

    public async Task<Result<string>> EvaluateTypedAsync(FunctionCallContext context, CancellationToken token)
    {
        context = ArgumentGuard.IsNotNull(context, nameof(context));

        var setterResult = await context.GetArgumentValueResultAsync(0, "setter", false, token).ConfigureAwait(false);
        if (!setterResult.IsSuccessful())
        {
            return Result.FromExistingResult<string>(setterResult);
        }

        var noAddPropertiesResult = setterResult.Value
            ? "{property.Name}()"
            : "_{property.Name.ToCamelCase()}";

        return await FunctionHelpers.ParseFromContextAsync(context, "NullCheck", c => Result.From(c.Settings.FluentBuilderMethods
            ? noAddPropertiesResult
            : "{property.Name}")).ConfigureAwait(false);
    }
}

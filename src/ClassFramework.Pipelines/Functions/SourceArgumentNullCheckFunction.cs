namespace ClassFramework.Pipelines.Functions;

public class SourceArgumentNullCheckFunction : IFunction<string>
{
    public async Task<Result<object?>> EvaluateAsync(FunctionCallContext context, CancellationToken token)
        => await EvaluateTypedAsync(context, token).ConfigureAwait(false);

    public async Task<Result<string>> EvaluateTypedAsync(FunctionCallContext context, CancellationToken token)
    {
        context = ArgumentGuard.IsNotNull(context, nameof(context));

        return await FunctionHelpers.ParseFromContextAsync(context, (contextBase, settings, classModel, property, isGenericArgument)
            => settings.AddNullChecks
                && settings.AddValidationCode() == ArgumentValidationType.None
                && !property.IsNullable
                && !property.IsValueType
                && !isGenericArgument // only if the source entity does not use validation...
                    ? $"if (source.{property.Name} {contextBase.NotNullCheck}) "
                    : string.Empty
            ).ConfigureAwait(false);
    }
}

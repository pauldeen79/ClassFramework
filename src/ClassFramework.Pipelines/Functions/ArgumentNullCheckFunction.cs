namespace ClassFramework.Pipelines.Functions;

public class ArgumentNullCheckFunction : IFunction<string>
{
    public async Task<Result<object?>> EvaluateAsync(FunctionCallContext context, CancellationToken token)
        => await EvaluateTypedAsync(context, token).ConfigureAwait(false);

    public async Task<Result<string>> EvaluateTypedAsync(FunctionCallContext context, CancellationToken token)
    {
        context = ArgumentGuard.IsNotNull(context, nameof(context));

        return await FunctionHelpers.ParseFromContextAsync(context, (contextBase, settings, classModel, property, isGenericArgument)
            => settings.AddNullChecks
                && !property.IsValueType
                && !property.IsNullable
                && !isGenericArgument
                    ? contextBase.CreateArgumentNullException(property.Name.ToCamelCase(context.Context.Settings.FormatProvider.ToCultureInfo()).GetCsharpFriendlyName())
                    : string.Empty
            ).ConfigureAwait(false);
    }
}

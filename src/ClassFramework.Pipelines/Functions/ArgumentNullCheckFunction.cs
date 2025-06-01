namespace ClassFramework.Pipelines.Functions;

public class ArgumentNullCheckFunction : IFunction<string>
{
    public async Task<Result<object?>> EvaluateAsync(FunctionCallContext context, CancellationToken token)
        => await EvaluateTypedAsync(context, token).ConfigureAwait(false);

    public async Task<Result<string>> EvaluateTypedAsync(FunctionCallContext context, CancellationToken token)
    {
        context = ArgumentGuard.IsNotNull(context, nameof(context));

        return (await new AsyncResultDictionaryBuilder()
            .Add(ResultNames.Settings, context.GetSettingsAsync())
            .Add(ResultNames.Property, context.Context.State.TryCastValueAsync<Property>(ResultNames.Property))
            .Add(ResultNames.Class, context.Context.State.TryCastValueAsync<ClassModel>(ResultNames.Class))
            .Add(ResultNames.Context, context.Context.State.TryCastValueAsync<ContextBase>(ResultNames.Context))
            .Build()
            .ConfigureAwait(false))
            .OnSuccess(results =>
            {
                var settings = results.GetValue<PipelineSettings>(ResultNames.Settings);
                var classModel = results.GetValue<ClassModel>(ResultNames.Class);
                var property = results.GetValue<Property>(ResultNames.Property);

                // note that for now, we assume that a generic type argument should not be included in argument null checks...
                // this might be the case (for example there is a constraint on class), but this is not supported yet
                var isGenericArgument = classModel.GetGenericTypeArguments().Contains(property.TypeName);

                return settings.AddNullChecks
                    && !property.IsValueType
                    && !property.IsNullable
                    && !isGenericArgument
                        ? results.GetValue<ContextBase>(ResultNames.Context).CreateArgumentNullException(property.Name.ToCamelCase(context.Context.Settings.FormatProvider.ToCultureInfo()).GetCsharpFriendlyName())
                        : string.Empty;
            });
    }
}

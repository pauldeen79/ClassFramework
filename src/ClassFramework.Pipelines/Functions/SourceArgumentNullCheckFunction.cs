namespace ClassFramework.Pipelines.Functions;

public class SourceArgumentNullCheckFunction : IFunction<string>
{
    public async Task<Result<object?>> EvaluateAsync(FunctionCallContext context, CancellationToken token)
        => await EvaluateTypedAsync(context, token).ConfigureAwait(false);

    public async Task<Result<string>> EvaluateTypedAsync(FunctionCallContext context, CancellationToken token)
    {
        context = ArgumentGuard.IsNotNull(context, nameof(context));

        return (await new AsyncResultDictionaryBuilder()
            .Add("settings", context.GetSettingsAsync())
            .Add("property", context.Context.State.TryCastValueAsync<Property>("property"))
            .Add("class", context.Context.State.TryCastValueAsync<ClassModel>("class"))
            .Add("context", context.Context.State.TryCastValueAsync<ContextBase>("context"))
            .Build()
            .ConfigureAwait(false))
            .OnSuccess(results =>
            {
                var settings = results.GetValue<PipelineSettings>("settings");
                var classModel = results.GetValue<ClassModel>("class");
                var property = results.GetValue<Property>("property");

                // note that for now, we assume that a generic type argument should not be included in argument null checks...
                // this might be the case (for example there is a constraint on class), but this is not supported yet
                var isGenericArgument = classModel.GetGenericTypeArguments().Contains(property.TypeName);

                return settings.AddNullChecks
                    && settings.AddValidationCode() == ArgumentValidationType.None
                    && !property.IsNullable
                    && !property.IsValueType
                    && !isGenericArgument // only if the source entity does not use validation...
                        ? $"if (source.{property.Name} {results.GetValue<ContextBase>("context").NotNullCheck}) "
                        : string.Empty;
            });
    }
}

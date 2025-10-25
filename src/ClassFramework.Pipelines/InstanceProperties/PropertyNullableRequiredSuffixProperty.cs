namespace ClassFramework.Pipelines.InstanceProperties;

[MemberName("NullableRequiredSuffix")]
[MemberInstanceType(typeof(Property))]
[MemberResultType(typeof(string))]
public class PropertyNullableRequiredSuffixProperty : IProperty
{
    public async Task<Result<object?>> EvaluateAsync(FunctionCallContext context, CancellationToken token)
    {
        context = ArgumentGuard.IsNotNull(context, nameof(context));

        return (await new AsyncResultDictionaryBuilder()
            .Add(Constants.Instance, context.GetInstanceValueResult<Property>())
            .Add(ResultNames.Settings, () => context.GetSettingsAsync())
            .BuildAsync()
            .ConfigureAwait(false))
            .OnSuccess<object?>(results => GetNullableRequiredSuffix(results.GetValue<PipelineSettings>(ResultNames.Settings), results.GetValue<Property>(Constants.Instance)));
    }

    private static string GetNullableRequiredSuffix(PipelineSettings settings, Property property)
        => !settings.AddNullChecks && !property.IsValueType && !property.IsNullable && settings.EnableNullableReferenceTypes
            ? "!"
            : string.Empty;
}

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
            .Add("instance", context.GetInstanceValueResult<Property>())
            .Add("settings", context.GetSettingsAsync())
            .Build()
            .ConfigureAwait(false))
            .OnSuccess<object?>(results => GetNullableRequiredSuffix(results.GetValue<PipelineSettings>("settings"), results.GetValue<Property>("instance")));
    }

    private static string GetNullableRequiredSuffix(PipelineSettings settings, Property property)
        => !settings.AddNullChecks && !property.IsValueType && !property.IsNullable && settings.EnableNullableReferenceTypes
            ? "!"
            : string.Empty;
}

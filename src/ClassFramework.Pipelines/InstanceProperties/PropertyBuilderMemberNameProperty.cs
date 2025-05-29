namespace ClassFramework.Pipelines.InstanceProperties;

[MemberName("BuilderMemberName")]
[MemberInstanceType(typeof(Property))]
[MemberResultType(typeof(string))]
public class PropertyBuilderMemberNameProperty : IProperty
{
    public async Task<Result<object?>> EvaluateAsync(FunctionCallContext context, CancellationToken token)
    {
        context = ArgumentGuard.IsNotNull(context, nameof(context));

        return (await new AsyncResultDictionaryBuilder()
            .Add("instance", context.GetInstanceValueResult<Property>())
            .Add("settings", context.GetSettingsAsync())
            .Build()
            .ConfigureAwait(false))
            .OnSuccess(results => Result.Success<object?>(results
                .GetValue<Property>("instance")
                .GetBuilderMemberName(results.GetValue<PipelineSettings>("settings"), context.Context.Settings.FormatProvider.ToCultureInfo())));
    }
}

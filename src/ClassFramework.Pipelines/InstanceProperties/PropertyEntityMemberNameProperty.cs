namespace ClassFramework.Pipelines.InstanceProperties;

[MemberName("EntityMemberName")]
[MemberInstanceType(typeof(Property))]
[MemberResultType(typeof(string))]
public class PropertyEntityMemberNameProperty : IProperty
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
                .GetEntityMemberName(results.GetValue<PipelineSettings>("settings").AddBackingFields || results.GetValue<PipelineSettings>("settings").CreateAsObservable, context.Context.Settings.FormatProvider.ToCultureInfo())));
    }
}

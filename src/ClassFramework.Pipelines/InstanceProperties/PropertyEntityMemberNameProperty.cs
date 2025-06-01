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
            .Add(Constants.Instance, context.GetInstanceValueResult<Property>())
            .Add(ResultNames.Settings, context.GetSettingsAsync())
            .Build()
            .ConfigureAwait(false))
            .OnSuccess<object?>(results => results
                .GetValue<Property>(Constants.Instance)
                .GetEntityMemberName(results.GetValue<PipelineSettings>(ResultNames.Settings).AddBackingFields || results.GetValue<PipelineSettings>(ResultNames.Settings).CreateAsObservable, context.Context.Settings.FormatProvider.ToCultureInfo()));
    }
}

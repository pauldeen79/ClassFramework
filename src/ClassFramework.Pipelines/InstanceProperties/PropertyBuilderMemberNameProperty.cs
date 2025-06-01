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
            .Add(Constants.Instance, context.GetInstanceValueResult<Property>())
            .Add(ResultNames.Settings, context.GetSettingsAsync())
            .Build()
            .ConfigureAwait(false))
            .OnSuccess<object?>(results => results
                .GetValue<Property>(Constants.Instance)
                .GetBuilderMemberName(results.GetValue<PipelineSettings>(ResultNames.Settings), context.Context.Settings.FormatProvider.ToCultureInfo()));
    }
}

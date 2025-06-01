namespace ClassFramework.Pipelines.InstanceProperties;

[MemberName(nameof(Property.TypeName))]
[MemberInstanceType(typeof(Property))]
[MemberResultType(typeof(string))]
public class PropertyTypeNameProperty : IProperty
{
    public async Task<Result<object?>> EvaluateAsync(FunctionCallContext context, CancellationToken token)
    {
        context = ArgumentGuard.IsNotNull(context, nameof(context));

        return (await new AsyncResultDictionaryBuilder()
            .Add("instance", context.GetInstanceValueResult<Property>())
            .Add("mappedContextBase", context.GetMappedContextBaseAsync())
            .Build()
            .ConfigureAwait(false))
            .OnSuccess(results => results.GetValue<MappedContextBase>("mappedContextBase").MapTypeName(results.GetValue<Property>("instance").TypeName));
    }
}

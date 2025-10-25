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
            .Add(Constants.Instance, context.GetInstanceValueResult<Property>())
            .Add(ResultNames.Context, () => context.GetMappedContextBaseAsync())
            .BuildAsync()
            .ConfigureAwait(false))
            .OnSuccess(results => results.GetValue<MappedContextBase>(ResultNames.Context).MapTypeName(results.GetValue<Property>(Constants.Instance).TypeName));
    }
}

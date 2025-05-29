namespace ClassFramework.Pipelines.InstanceProperties;

[MemberName("DefaultValue")]
[MemberInstanceType(typeof(Property))]
[MemberResultType(typeof(string))]
public class PropertyDefaultValueProperty : IProperty
{
    private readonly ICsharpExpressionDumper _csharpExpressionDumper;

    public PropertyDefaultValueProperty(ICsharpExpressionDumper csharpExpressionDumper)
    {
        ArgumentGuard.IsNotNull(csharpExpressionDumper, nameof(csharpExpressionDumper));

        _csharpExpressionDumper = csharpExpressionDumper;
    }

    public async Task<Result<object?>> EvaluateAsync(FunctionCallContext context, CancellationToken token)
    {
        context = ArgumentGuard.IsNotNull(context, nameof(context));

        return (await new AsyncResultDictionaryBuilder()
            .Add("instance", context.GetInstanceValueResult<Property>())
            .Add("typeName", context.GetTypeNameAsync())
            .Add("mappedContextBase", context.GetMappedContextBaseAsync())
            .Build()
            .ConfigureAwait(false))
            .OnSuccess<object?>(results => results
                .GetValue<Property>("instance")
                .GetDefaultValue(_csharpExpressionDumper, results.GetValue<string>("typeName"), results.GetValue<MappedContextBase>("mappedContextBase")));
    }
}

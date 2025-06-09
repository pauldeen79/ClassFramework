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

        return await (await new AsyncResultDictionaryBuilder()
            .Add(Constants.Instance, context.GetInstanceValueResult<Property>())
            .Add(ResultNames.TypeName, context.GetTypeNameAsync())
            .Add(ResultNames.Context, context.GetMappedContextBaseAsync())
            .Build()
            .ConfigureAwait(false))
            .OnSuccess(async results =>
            {
                var defaultValue = results
                    .GetValue<Property>(Constants.Instance)
                    .GetDefaultValue(_csharpExpressionDumper, results.GetValue<string>(ResultNames.TypeName), results.GetValue<MappedContextBase>(ResultNames.Context));

                return await context.Context.EvaluateTypedAsync<GenericFormattableString>($"$\"{defaultValue}\"", token).ConfigureAwait(false);
            }).ConfigureAwait(false);
    }
}

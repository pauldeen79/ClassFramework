namespace ClassFramework.Pipelines.InstanceProperties;

[MemberName("BuilderFuncPrefix")]
[MemberInstanceType(typeof(Property))]
[MemberResultType(typeof(string))]
public class PropertyBuilderFuncPrefixProperty : IProperty
{
    public async Task<Result<object?>> EvaluateAsync(FunctionCallContext context, CancellationToken token)
    {
        context = ArgumentGuard.IsNotNull(context, nameof(context));

        return await context.EvaluateForProperty(
            (property, settings) =>
            {
                //TODO: Check if we can get access to mapping medatadata here...
                //var metadata = context.Request.GetMappingMetadata(property.TypeName).ToArray();
                //var builderName = metadata.GetStringValue(MetadataNames.CustomBuilderName, DefaultBuilderName);
                //var useBuilderLazyValues = settings.UseBuilderLazyValues && builderName == DefaultBuilderName;
                var useBuilderLazyValues = settings.UseBuilderLazyValues;
                return useBuilderLazyValues
                    ? $"new {property.TypeName.ToLazy()}(() => "
                    : string.Empty;
            }).ConfigureAwait(false);
    }
}

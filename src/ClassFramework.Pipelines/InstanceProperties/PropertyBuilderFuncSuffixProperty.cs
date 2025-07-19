namespace ClassFramework.Pipelines.InstanceProperties;

[MemberName("BuilderFuncSuffix")]
[MemberInstanceType(typeof(Property))]
[MemberResultType(typeof(string))]
public class PropertyBuilderFuncSuffixProperty : IProperty
{
    public async Task<Result<object?>> EvaluateAsync(FunctionCallContext context, CancellationToken token)
    {
        context = ArgumentGuard.IsNotNull(context, nameof(context));

        return await context.EvaluateForProperty(
            (property, settings, mappedContextBase) =>
            {
                var metadata = mappedContextBase.GetMappingMetadata(property.TypeName).ToArray();
                var builderName = metadata.GetStringValue(MetadataNames.CustomBuilderName, ContextBase.DefaultBuilderName);
                var useBuilderLazyValues = settings.UseBuilderLazyValues && builderName == ContextBase.DefaultBuilderName;
                return useBuilderLazyValues
                    ? ")"
                    : string.Empty;
            }).ConfigureAwait(false);
    }
}

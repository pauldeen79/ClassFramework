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
            (property, settings, mappedContextBase) =>
            {
                var metadata = mappedContextBase.GetMappingMetadata(property.TypeName).ToArray();
                var builderName = metadata.GetStringValue(MetadataNames.CustomBuilderName, ContextBase.DefaultBuilderName);
                var useBuilderLazyValues = settings.UseBuilderLazyValues && builderName == ContextBase.DefaultBuilderName;
                var typeName = property.TypeName.FixTypeName().IsCollectionTypeName()
                    ? property.TypeName.FixTypeName().GetCollectionItemType()
                    : property.TypeName.FixTypeName();
                return useBuilderLazyValues
                    ? $"new {typeName.ToLazy()}(() => "
                    : string.Empty;
            }).ConfigureAwait(false);
    }
}

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
                var useBuilderLazyValues = mappedContextBase.UseBuilderLazyValues(property.TypeName);
                var typeName = property.TypeName.FixTypeName().IsCollectionTypeName()
                    ? property.TypeName.FixTypeName().GetCollectionItemType()
                    : property.TypeName.FixTypeName();
                
                return useBuilderLazyValues.GetLazyPrefix(typeName);
            }).ConfigureAwait(false);
    }
}

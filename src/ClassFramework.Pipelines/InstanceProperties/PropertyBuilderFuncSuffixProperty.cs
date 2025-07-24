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
            (property, settings, mappedContextBase)
                => mappedContextBase.UseBuilderLazyValues(property.TypeName).GetLazySuffix()).ConfigureAwait(false);
    }
}

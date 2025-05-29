namespace ClassFramework.Pipelines.InstanceProperties;

[MemberName(nameof(Property.TypeName))]
[MemberInstanceType(typeof(Property))]
[MemberResultType(typeof(string))]
public class PropertyTypeNameProperty : IProperty
{
    public Task<Result<object?>> EvaluateAsync(FunctionCallContext context, CancellationToken token)
        => Task.Run(() =>
        {
            context = ArgumentGuard.IsNotNull(context, nameof(context));

            return context.GetInstanceValueResult<Property>()
                .Transform<object?>(property => property.TypeName);
        }, token);
}

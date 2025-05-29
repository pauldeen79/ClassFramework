namespace ClassFramework.Pipelines.InstanceProperties;

[MemberName(nameof(Property.Name))]
[MemberInstanceType(typeof(Property))]
[MemberResultType(typeof(string))]
public class PropertyNameProperty : IProperty
{
    public Task<Result<object?>> EvaluateAsync(FunctionCallContext context, CancellationToken token)
        => Task.Run(() =>
        {
            context = ArgumentGuard.IsNotNull(context, nameof(context));

            return context.GetInstanceValueResult<Property>()
                .Transform<object?>(property => property.Name);
        }, token);
}

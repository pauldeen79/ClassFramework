namespace ClassFramework.Pipelines.InstanceProperties;

[MemberName("EntityMemberName")]
[MemberInstanceType(typeof(Property))]
[MemberResultType(typeof(string))]
public class PropertyEntityMemberNameProperty : IProperty
{
    public async Task<Result<object?>> EvaluateAsync(FunctionCallContext context, CancellationToken token)
    {
        context = ArgumentGuard.IsNotNull(context, nameof(context));

        return await context.EvaluateForProperty(
            (property, settings) => property.GetEntityMemberName(settings.AddBackingFields || settings.CreateAsObservable, context.Context.Settings.FormatProvider.ToCultureInfo())).ConfigureAwait(false);
    }
}

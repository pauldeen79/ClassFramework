namespace ClassFramework.Pipelines.InstanceProperties;

[MemberName("BuilderMemberName")]
[MemberInstanceType(typeof(Property))]
[MemberResultType(typeof(string))]
public class PropertyBuilderMemberNameProperty : IProperty
{
    public async Task<Result<object?>> EvaluateAsync(FunctionCallContext context, CancellationToken token)
    {
        context = ArgumentGuard.IsNotNull(context, nameof(context));

        return await context.EvaluateForProperty(
            (property, settings) => property.GetBuilderMemberName(settings, context.Context.Settings.FormatProvider.ToCultureInfo())).ConfigureAwait(false);
    }
}

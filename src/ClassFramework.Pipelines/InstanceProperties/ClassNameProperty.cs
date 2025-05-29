namespace ClassFramework.Pipelines.InstanceProperties;

[MemberName(nameof(ClassModel.Name))]
[MemberInstanceType(typeof(ClassModel))]
[MemberResultType(typeof(string))]
public class ClassNameProperty : IProperty
{
    public Task<Result<object?>> EvaluateAsync(FunctionCallContext context, CancellationToken token)
        => Task.Run(() =>
        {
            context = ArgumentGuard.IsNotNull(context, nameof(context));

            return context.GetInstanceValueResult<ClassModel>().Transform<object?>(result => result.Name);
        }, token);
}

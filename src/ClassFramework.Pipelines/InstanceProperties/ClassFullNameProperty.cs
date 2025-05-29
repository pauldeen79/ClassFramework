namespace ClassFramework.Pipelines.InstanceProperties;

[MemberName(nameof(ClassModel.FullName))]
[MemberInstanceType(typeof(ClassModel))]
[MemberResultType(typeof(string))]
public class ClassFullNameProperty : IProperty
{
    public Task<Result<object?>> EvaluateAsync(FunctionCallContext context, CancellationToken token)
        => Task.Run(() =>
        {
            context = ArgumentGuard.IsNotNull(context, nameof(context));

            return context.GetInstanceValueResult<ClassModel>()
                .Transform<object?>(classModel => classModel.FullName.WithoutTypeGenerics());
        }, token);
}

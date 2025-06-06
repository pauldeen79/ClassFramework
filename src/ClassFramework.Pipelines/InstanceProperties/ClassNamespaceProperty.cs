namespace ClassFramework.Pipelines.InstanceProperties;

[MemberName(nameof(ClassModel.Namespace))]
[MemberInstanceType(typeof(ClassModel))]
[MemberResultType(typeof(string))]
public class ClassNamespaceProperty : IProperty
{
    public Task<Result<object?>> EvaluateAsync(FunctionCallContext context, CancellationToken token)
        => Task.Run(() =>
        {
            context = ArgumentGuard.IsNotNull(context, nameof(context));

            return context.GetInstanceValueResult<ClassModel>()
                .Transform<object?>(classModel => classModel.Namespace);
        }, token);
}

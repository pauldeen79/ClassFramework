namespace ClassFramework.Pipelines.Functions;

[MemberArgument(Constants.Expression, typeof(string))]
public class CsharpFriendlyTypeNameFunction : IFunction<string>
{
    public async Task<Result<object?>> EvaluateAsync(FunctionCallContext context, CancellationToken token)
        => await EvaluateTypedAsync(context, token).ConfigureAwait(false);

    public Task<Result<string>> EvaluateTypedAsync(FunctionCallContext context, CancellationToken token)
        => FunctionHelpers.ParseFromStringArgumentAsync(context.IsNotNull(nameof(context)), "CsharpFriendlyTypeName", s => Result.Success(s.GetCsharpFriendlyTypeName()), token);
}

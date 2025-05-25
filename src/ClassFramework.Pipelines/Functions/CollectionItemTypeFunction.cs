namespace ClassFramework.Pipelines.Functions;

public class CollectionItemTypeFunction : IFunction
{
    public Task<Result<object?>> EvaluateAsync(FunctionCallContext context, CancellationToken token)
        => FunctionHelpers.ParseFromStringArgument(context, "CollectionItemType", s => Result.Success<object?>(s.GetCollectionItemType()), token);
}

namespace ClassFramework.Pipelines.Functions;

public class CollectionItemTypeFunction : IFunction
{
    public Result<object?> Evaluate(FunctionCallContext context)
        => FunctionBase.ParseFromStringArgument(context, "CollectionItemType", s => Result.Success<object?>(s.GetCollectionItemType()));

    public Result Validate(FunctionCallContext context)
        => Result.Success();
}

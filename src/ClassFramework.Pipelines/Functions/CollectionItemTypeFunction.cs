namespace ClassFramework.Pipelines.Functions;

public class CollectionItemTypeFunction : IFunction
{
    public Result<object?> Evaluate(FunctionCallContext context)
        => FunctionHelpers.ParseFromStringArgument(context, "CollectionItemType", s => Result.Success<object?>(s.GetCollectionItemType()));
}

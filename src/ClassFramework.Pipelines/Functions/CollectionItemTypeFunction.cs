﻿namespace ClassFramework.Pipelines.Functions;

public class CollectionItemTypeFunction : IFunction
{
    public Result<object?> Parse(FunctionParseResult functionParseResult, object? context, IFunctionParseResultEvaluator evaluator, IExpressionParser parser)
        => FunctionBase.ParseFromStringArgument(functionParseResult, context, evaluator, parser, "CollectionItemType", s => Result.Success<object?>(s.GetCollectionItemType()));
}

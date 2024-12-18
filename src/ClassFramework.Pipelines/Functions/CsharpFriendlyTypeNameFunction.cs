﻿namespace ClassFramework.Pipelines.Functions;

public class CsharpFriendlyTypeNameFunction : IFunctionResultParser
{
    public Result<object?> Parse(FunctionParseResult functionParseResult, object? context, IFunctionParseResultEvaluator evaluator, IExpressionParser parser)
        => FunctionBase.ParseFromStringArgument(functionParseResult, context, evaluator, parser, "CsharpFriendlyTypeName", s => Result.Success<object?>(s.GetCsharpFriendlyTypeName()));
}

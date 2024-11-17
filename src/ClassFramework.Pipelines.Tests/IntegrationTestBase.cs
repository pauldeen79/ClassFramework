namespace ClassFramework.Pipelines.Tests;

public abstract class IntegrationTestBase<T> : TestBase
    where T : class
{
    protected T CreateSut() => Scope!.ServiceProvider.GetRequiredService<T>();

    protected IntegrationTestBase()
    {
        Provider = new ServiceCollection()
            .AddParsers()
            .AddClassFrameworkPipelines()
            .AddCsharpExpressionDumper()
            .AddSingleton<IFunctionResultParser, PropertyNameResultParser>()
            .AddSingleton<IFunctionResultParser, ToCamelCaseResultParser>()
            .AddSingleton<IVariable, PropertyNameVariable>()
            .BuildServiceProvider();
        Scope = Provider.CreateScope();
    }

    private sealed class PropertyNameResultParser : IFunctionResultParser
    {
        public Result<object?> Parse(FunctionParseResult functionParseResult, object? context, IFunctionParseResultEvaluator evaluator, IExpressionParser parser)
        {
            if (functionParseResult.FunctionName != "PropertyName")
            {
                return Result.Continue<object?>();
            }

            if (context is PropertyContext propertyContext)
            {
                return Result.Success<object?>(propertyContext.SourceModel.Name);
            }

            return Result.Invalid<object?>("Could not get property name from context, because the context is not of type PropertyContext");
        }
    }

    private sealed class ToCamelCaseResultParser : IFunctionResultParser
    {
        public Result<object?> Parse(FunctionParseResult functionParseResult, object? context, IFunctionParseResultEvaluator evaluator, IExpressionParser parser)
        {
            if (functionParseResult.FunctionName != "ToCamelCase")
            {
                return Result.Continue<object?>();
            }

            var valueResult = functionParseResult.Arguments.First().GetValueResult(context, evaluator, parser, functionParseResult.FormatProvider);
            if (!valueResult.IsSuccessful())
            {
                return valueResult;
            }

            return Result.Success<object?>(valueResult.Value.ToStringWithDefault().ToCamelCase(functionParseResult.FormatProvider.ToCultureInfo()));
        }
    }

    private sealed class PropertyNameVariable : IVariable
    {
        public Result<object?> Process(string variable, object? context)
        {
            if (variable == "property.Name")
            {
                if (context is PropertyContext propertyContext)
                {
                    return Result.Success<object?>(propertyContext.SourceModel.Name);
                }

                return Result.Invalid<object?>("Could not get property name from context, because the context is not of type PropertyContext");
            }

            return Result.Continue<object?>();
        }
    }
}

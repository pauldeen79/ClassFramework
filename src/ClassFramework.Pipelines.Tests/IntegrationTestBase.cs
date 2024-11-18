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
            .AddExpressionParser()
            .AddSingleton<IFunctionResultParser, PropertyNameResultParser>()
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
}

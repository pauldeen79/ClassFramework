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
            .AddSingleton<IFunction, PropertyNameResultParser>()
            .BuildServiceProvider();
        Scope = Provider.CreateScope();
    }

    private sealed class PropertyNameResultParser : IFunction
    {
        public Result<object?> Evaluate(FunctionCallContext context)
        {
            if (context.Context is PropertyContext propertyContext)
            {
                return Result.Success<object?>(propertyContext.SourceModel.Name);
            }

            return Result.Invalid<object?>("Could not get property name from context, because the context is not of type PropertyContext");
        }

        public Result Validate(FunctionCallContext context)
            => Result.Success();
    }
}

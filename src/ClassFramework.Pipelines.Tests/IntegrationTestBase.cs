namespace ClassFramework.Pipelines.Tests;

public abstract class IntegrationTestBase<T> : TestBase
    where T : class
{
    protected T CreateSut() => Scope!.ServiceProvider.GetRequiredService<T>();

    protected IntegrationTestBase()
    {
        Provider = new ServiceCollection()
            .AddExpressionEvaluator()
            .AddClassFrameworkPipelines()
            .AddCsharpExpressionDumper()
            .AddSingleton<IFunction, PropertyNameResultParser>()
            .BuildServiceProvider();
        Scope = Provider.CreateScope();
    }

    private sealed class PropertyNameResultParser : IFunction
    {
        public async Task<Result<object?>> EvaluateAsync(FunctionCallContext context, CancellationToken token)
        {
            var ctx = await context.Context.State["context"].ConfigureAwait(false);
            if (ctx.GetValueOrThrow() is PropertyContext propertyContext)
            {
                return Result.Success<object?>(propertyContext.SourceModel.Name);
            }

            return Result.Invalid<object?>("Could not get property name from context, because the context is not of type PropertyContext");
        }
    }
}

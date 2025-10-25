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
            .BuildServiceProvider();
        Scope = Provider.CreateScope();
    }
}

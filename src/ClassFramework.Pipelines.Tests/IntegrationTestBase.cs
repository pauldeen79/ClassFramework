
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
            .AddScoped<IPipelineComponentInterceptor>(_ => new FixBuilderInterceptor())
            .BuildServiceProvider();
        Scope = Provider.CreateScope();
    }
}

public class FixBuilderInterceptor : IPipelineComponentInterceptor
{
    public Task<Result> ExecuteAsync<TCommand>(TCommand command, ICommandService commandService, Func<Task<Result>> next, CancellationToken token)
    {
        next = ArgumentGuard.IsNotNull(next, nameof(next));

        return next();
    }

    public Task<Result> ExecuteAsync<TCommand, TResponse>(TCommand command, TResponse response, ICommandService commandService, Func<Task<Result>> next, CancellationToken token)
    {
        next = ArgumentGuard.IsNotNull(next, nameof(next));

        if (response is TypeBaseBuilder typeBaseBuilder && string.IsNullOrEmpty(typeBaseBuilder.Name))
        {
            typeBaseBuilder.Name = "Dummy";
        }

        return next();
    }
}

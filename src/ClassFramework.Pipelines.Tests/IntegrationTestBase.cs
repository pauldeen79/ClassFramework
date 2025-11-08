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
            .AddScoped<IPipelineComponentDecorator>(_ => new FixBuilderDecorator())
            .BuildServiceProvider();
        Scope = Provider.CreateScope();
    }
}

public class FixBuilderDecorator : IPipelineComponentDecorator
{
    public Task<Result> ExecuteAsync<TCommand>(IPipelineComponent<TCommand> component, TCommand command, ICommandService commandService, CancellationToken token)
    {
        component = ArgumentGuard.IsNotNull(component, nameof(component));

        return component.ExecuteAsync(command, commandService, token);
    }

    public Task<Result> ExecuteAsync<TCommand, TResponse>(IPipelineComponent<TCommand, TResponse> component, TCommand command, TResponse response, ICommandService commandService, CancellationToken token)
    {
        component = ArgumentGuard.IsNotNull(component, nameof(component));

        if (response is TypeBaseBuilder typeBaseBuilder && string.IsNullOrEmpty(typeBaseBuilder.Name))
        {
            typeBaseBuilder.Name = "Dummy";
        }

        return component.ExecuteAsync(command, response, commandService, token);
    }
}

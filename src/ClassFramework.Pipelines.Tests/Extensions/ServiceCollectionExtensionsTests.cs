namespace ClassFramework.Pipelines.Tests.Extensions;

public class ServiceCollectionExtensionsTests : TestBase
{
    public class AddPipelines : ServiceCollectionExtensionsTests
    {
        [Fact]
        public void Registers_All_Required_Dependencies()
        {
            // Arrange
            var serviceCollection = new ServiceCollection()
                .AddScoped(_ => Fixture.Freeze<IExpressionEvaluator>()) // note that normally, you would probably use AddParsers from the CrossCutting.Utilities.Parsers package...
                .AddCsharpExpressionDumper()
                .AddClassFrameworkPipelines();

            // Act & Assert
            Action a = () =>
            {
                using var provder = serviceCollection.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });
            };
            a.ShouldNotThrow();
        }

        [Fact]
        public void Can_Resolve_BuilderPipeline()
        {
            // Arrange
            var serviceCollection = new ServiceCollection()
                .AddScoped(_ => Fixture.Freeze<IExpressionEvaluator>()) // note that normally, you would probably use AddParsers from the CrossCutting.Utilities.Parsers package...
                .AddScoped(_ => Fixture.Freeze<ICsharpExpressionDumper>())  // note that normally, you would probably use AddCsharpExpressionDumper from the CsharpDumper package...
                .AddClassFrameworkPipelines();
            using var provider = serviceCollection.BuildServiceProvider();
            using var scope = provider.CreateScope();

            // Act
            var builder = scope.ServiceProvider.GetRequiredService<IPipeline<BuilderContext>>();

            // Assert
            builder.ShouldBeOfType<Pipeline<BuilderContext>>();
        }

        [Fact]
        public void Can_Resolve_EntityPipeline()
        {
            // Arrange
            var serviceCollection = new ServiceCollection()
                .AddScoped(_ => Fixture.Freeze<IExpressionEvaluator>()) // note that normally, you would probably use AddParsers from the CrossCutting.Utilities.Parsers package...
                .AddClassFrameworkPipelines();
            using var provider = serviceCollection.BuildServiceProvider();
            using var scope = provider.CreateScope();

            // Act
            var builder = scope.ServiceProvider.GetRequiredService<IPipeline<EntityContext>>();

            // Assert
            builder.ShouldBeOfType<Pipeline<EntityContext>>();
        }

        [Fact]
        public void Can_Resolve_ReflectionPipeline()
        {
            // Arrange
            var serviceCollection = new ServiceCollection()
                .AddScoped(_ => Fixture.Freeze<IExpressionEvaluator>()) // note that normally, you would probably use AddParsers from the CrossCutting.Utilities.Parsers package...
                .AddClassFrameworkPipelines();
            using var provider = serviceCollection.BuildServiceProvider();
            using var scope = provider.CreateScope();

            // Act
            var builder = scope.ServiceProvider.GetRequiredService<IPipeline<ReflectionContext>>();

            // Assert
            builder.ShouldBeOfType<Pipeline<ReflectionContext>>();
        }
    }
}

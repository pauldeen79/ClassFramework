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
                .AddScoped(_ => Fixture.Freeze<IFormattableStringParser>()) // note that normally, you would probably use AddParsers from the CrossCutting.Utilities.Parsers package...
                .AddCsharpExpressionDumper()
                .AddPipelines();

            // Act & Assert
            serviceCollection.Invoking(x =>
            {
                using var provder = x.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });
            }).Should().NotThrow();
        }

        [Fact]
        public void Can_Resolve_BuilderPipelineBuilder()
        {
            // Arrange
            var serviceCollection = new ServiceCollection()
                .AddScoped(_ => Fixture.Freeze<IFormattableStringParser>()) // note that normally, you would probably use AddParsers from the CrossCutting.Utilities.Parsers package...
                .AddPipelines();
            using var provider = serviceCollection.BuildServiceProvider();
            using var scope = provider.CreateScope();

            // Act
            var builder = scope.ServiceProvider.GetRequiredService<IPipelineBuilder<IConcreteTypeBuilder, BuilderContext>>();

            // Assert
            builder.Should().BeOfType<Pipelines.Builder.PipelineBuilder>();
        }

        [Fact]
        public void Can_Resolve_BuilderPipeline()
        {
            // Arrange
            var serviceCollection = new ServiceCollection()
                .AddScoped(_ => Fixture.Freeze<IFormattableStringParser>()) // note that normally, you would probably use AddParsers from the CrossCutting.Utilities.Parsers package...
                .AddPipelines();
            using var provider = serviceCollection.BuildServiceProvider();
            using var scope = provider.CreateScope();

            // Act
            var builder = scope.ServiceProvider.GetRequiredService<IPipeline<IConcreteTypeBuilder, BuilderContext>>();

            // Assert
            builder.Should().BeOfType<Pipeline<IConcreteTypeBuilder, BuilderContext>>();
        }

        [Fact]
        public void Can_Resolve_EntityPipelineBuilder()
        {
            // Arrange
            var serviceCollection = new ServiceCollection()
                .AddScoped(_ => Fixture.Freeze<IFormattableStringParser>()) // note that normally, you would probably use AddParsers from the CrossCutting.Utilities.Parsers package...
                .AddPipelines();
            using var provider = serviceCollection.BuildServiceProvider();
            using var scope = provider.CreateScope();

            // Act
            var builder = scope.ServiceProvider.GetRequiredService<IPipelineBuilder<IConcreteTypeBuilder, EntityContext>>();

            // Assert
            builder.Should().BeOfType<Pipelines.Entity.PipelineBuilder>();
        }

        [Fact]
        public void Can_Resolve_EntityPipeline()
        {
            // Arrange
            var serviceCollection = new ServiceCollection()
                .AddScoped(_ => Fixture.Freeze<IFormattableStringParser>()) // note that normally, you would probably use AddParsers from the CrossCutting.Utilities.Parsers package...
                .AddPipelines();
            using var provider = serviceCollection.BuildServiceProvider();
            using var scope = provider.CreateScope();

            // Act
            var builder = scope.ServiceProvider.GetRequiredService<IPipeline<IConcreteTypeBuilder, EntityContext>>();

            // Assert
            builder.Should().BeOfType<Pipeline<IConcreteTypeBuilder, EntityContext>>();
        }

        [Fact]
        public void Can_Resolve_ReflectionPipelineBuilder()
        {
            // Arrange
            var serviceCollection = new ServiceCollection()
                .AddScoped(_ => Fixture.Freeze<IFormattableStringParser>()) // note that normally, you would probably use AddParsers from the CrossCutting.Utilities.Parsers package...
                .AddPipelines();
            using var provider = serviceCollection.BuildServiceProvider();
            using var scope = provider.CreateScope();

            // Act
            var builder = scope.ServiceProvider.GetRequiredService<IPipelineBuilder<TypeBaseBuilder, ReflectionContext>>();

            // Assert
            builder.Should().BeOfType<Pipelines.Reflection.PipelineBuilder>();
        }

        [Fact]
        public void Can_Resolve_ReflectionPipeline()
        {
            // Arrange
            var serviceCollection = new ServiceCollection()
                .AddScoped(_ => Fixture.Freeze<IFormattableStringParser>()) // note that normally, you would probably use AddParsers from the CrossCutting.Utilities.Parsers package...
                .AddPipelines();
            using var provider = serviceCollection.BuildServiceProvider();
            using var scope = provider.CreateScope();

            // Act
            var builder = scope.ServiceProvider.GetRequiredService<IPipeline<TypeBaseBuilder, ReflectionContext>>();

            // Assert
            builder.Should().BeOfType<Pipeline<TypeBaseBuilder, ReflectionContext>>();
        }
    }
}

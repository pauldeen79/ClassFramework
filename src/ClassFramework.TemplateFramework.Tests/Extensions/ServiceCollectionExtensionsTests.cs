namespace ClassFramework.TemplateFramework.Tests.Extensions;

public class ServiceCollectionExtensionsTests
{
    public class AddClassFrameworkTemplates : ServiceCollectionExtensionsTests
    {
        [Fact]
        public void Registers_All_Required_Dependencies()
        {
            // Arrange
            var serviceCollection = new ServiceCollection()
                .AddCsharpExpressionDumper() // dependency of some templates
                .AddParsers()
                .AddPipelines()
                .AddClassFrameworkTemplates()
                .AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CsharpClassGeneratorPipelineCodeGenerationProviderBase).Assembly))
                ;

            // Act & Assert
            serviceCollection.Invoking(x =>
            {
                using var provder = x.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });
            }).Should().NotThrow();
        }
    }
}

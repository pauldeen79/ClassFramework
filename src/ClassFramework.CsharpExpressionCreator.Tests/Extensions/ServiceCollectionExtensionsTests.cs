namespace ClassFramework.CsharpExpressionCreator.Tests.Extensions;

public class ServiceCollectionExtensionsTests
{
    public class AddCsharpExpressionCreator : ServiceCollectionExtensionsTests
    {
        [Fact]
        public void Registers_All_Required_Dependencies()
        {
            // Arrange
            var serviceCollection = new ServiceCollection().AddCsharpExpressionCreator();

            // Act & Assert
            serviceCollection.Invoking(x =>
            {
                using var provder = x.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });
            }).Should().NotThrow();
        }
    }
}

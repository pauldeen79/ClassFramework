namespace ClassFramework.Pipelines.Tests.ObjectResolvers;

public class CultureInfoResolverTests : TestBase<CultureInfoResolver>
{
    public class Resolve : CultureInfoResolverTests
    {
        [Fact]
        public void Returns_Not_Supported_On_Unsupported_Context_Type()
        {
            // Arrange
            var sourceObject = new object();
            var sut = CreateSut();

            // Act
            var result = sut.Resolve<CultureInfo>(sourceObject);

            // Assert
            result.Status.Should().Be(ResultStatus.NotSupported);
            result.ErrorMessage.Should().Be("Could not get culture info from context, because the context type System.Object is not supported");
        }

        [Fact]
        public void Returns_Continue_When_Type_Is_Not_CultureInfo()
        {
            // Arrange
            var sourceObject = new object();
            var sut = CreateSut();

            // Act
            var result = sut.Resolve<ClassModelResolverTests>(sourceObject);

            // Assert
            result.Status.Should().Be(ResultStatus.Continue);
        }
    }
}

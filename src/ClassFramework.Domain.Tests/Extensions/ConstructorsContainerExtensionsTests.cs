namespace ClassFramework.Domain.Tests.Extensions;

public class ConstructorsContainerExtensionsTests : TestBase
{
    public class HasPublicParameterlessConstructor : ConstructorsContainerExtensionsTests
    {
        [Fact]
        public void Returns_True_When_No_Constructors_Are_Defined()
        {
            // Arrange
            var sut = Fixture.Freeze<IConstructorsContainer>();
            sut.Constructors.Returns(new List<Constructor>());

            // Act
            var result = sut.HasPublicParameterlessConstructor();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void Returns_True_When_A_Public_Constructor_Is_Defined_Without_Any_Parameters()
        {
            // Arrange
            var sut = Fixture.Freeze<IConstructorsContainer>();
            sut.Constructors.Returns([new Constructor(string.Empty, default, default, default, default, default, default, Domains.Visibility.Public, [], [], [], [])]);

            // Act
            var result = sut.HasPublicParameterlessConstructor();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void Returns_False_When_A_Private_Constructor_Is_Defined_Without_Any_Parameters()
        {
            // Arrange
            var sut = Fixture.Freeze<IConstructorsContainer>();
            sut.Constructors.Returns([new Constructor(string.Empty, default, default, default, default, default, default, Domains.Visibility.Private, [], [], [], [])]);

            // Act
            var result = sut.HasPublicParameterlessConstructor();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void Returns_False_When_A_Public_Constructor_Is_Defined_With_Parameter()
        {
            // Arrange
            var sut = Fixture.Freeze<IConstructorsContainer>();
            sut.Constructors.Returns([new Constructor(string.Empty, default, default, default, default, default, default, Domains.Visibility.Public, [], [], [new Parameter(default, default, default, "System.String", default, default, [], [], "arg", default)], [])]);

            // Act
            var result = sut.HasPublicParameterlessConstructor();

            // Assert
            result.Should().BeFalse();
        }
    }
}

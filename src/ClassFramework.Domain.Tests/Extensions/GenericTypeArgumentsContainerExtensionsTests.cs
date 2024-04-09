using NSubstitute;

namespace ClassFramework.Domain.Tests.Extensions;

public class GenericTypeArgumentsContainerExtensionsTests : TestBase
{
    public class GetGenericTypeArgumentsString : GenericTypeArgumentsContainerExtensionsTests
    {
        [Fact]
        public void Returns_Empty_String_When_No_GenericArguments_Are_Present()
        {
            // Arrange
            var sut = Fixture.Freeze<IGenericTypeArgumentsContainer>();
            sut.GenericTypeArguments.Returns(Array.Empty<TypeInfo>());

            // Act
            var result = sut.GetGenericTypeArgumentsString();

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void Returns_Correct_Result_When_GenericArguments_Are_Present()
        {
            // Arrange
            var sut = Fixture.Freeze<IGenericTypeArgumentsContainer>();
            var typeContainer = new TypeInfoBuilder().WithTypeName("T").Build();
            sut.GenericTypeArguments.Returns([typeContainer]);

            // Act
            var result = sut.GetGenericTypeArgumentsString();

            // Assert
            result.Should().Be("<T>");
        }
    }

    public class GetGenericTypeArgumentConstraintsString : GenericTypeArgumentsContainerExtensionsTests
    {
        [Fact]
        public void Returns_Empty_String_When_No_GenericArguments_Are_Present()
        {
            // Arrange
            var sut = Fixture.Freeze<IGenericTypeArgumentsContainer>();
            sut.GenericTypeArguments.Returns(Array.Empty<TypeInfo>());

            // Act
            var result = sut.GetGenericTypeArgumentConstraintsString(8);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void Returns_Empty_String_When_No_GenericArgumentConstraints_Are_Present()
        {
            // Arrange
            var sut = Fixture.Freeze<IGenericTypeArgumentsContainer>();
            var typeContainer = new TypeInfoBuilder().WithTypeName("T").Build();
            sut.GenericTypeArguments.Returns([typeContainer]);

            // Act
            var result = sut.GetGenericTypeArgumentConstraintsString(8);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void Returns_Correct_Result_When_GenericArguments_Are_Present()
        {
            // Arrange
            var sut = Fixture.Freeze<IGenericTypeArgumentsContainer>();
            var typeContainer = new TypeInfoBuilder().WithTypeName("T").Build();
            sut.GenericTypeArguments.Returns([typeContainer]);
            sut.GenericTypeArgumentConstraints.Returns(["where T : class"]);

            // Act
            var result = sut.GetGenericTypeArgumentConstraintsString(8);

            // Assert
            result.Should().Be(@"
        where T : class");
        }
    }
}

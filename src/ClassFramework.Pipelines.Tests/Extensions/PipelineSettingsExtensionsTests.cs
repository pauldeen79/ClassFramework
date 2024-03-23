namespace ClassFramework.Pipelines.Tests.Extensions;

public class PipelineSettingsExtensionsTests : TestBase<PipelineSettingsBuilder>
{
    public class AddValidationCode : PipelineSettingsExtensionsTests
    {
        [Fact]
        public void Returns_None_When_ValidateArguments_Is_None()
        {
            // Arrange
            var sut = CreateSut().WithValidateArguments(ArgumentValidationType.None).Build();

            // Act
            var result = sut.AddValidationCode();

            // Assert
            result.Should().Be(ArgumentValidationType.None);
        }

        [Theory]
        [InlineData(ArgumentValidationType.IValidatableObject)]
        [InlineData(ArgumentValidationType.None)]
        public void Returns_ValidateArguments_When_EnableInheritance_Is_False(ArgumentValidationType input)
        {
            // Arrange
            var sut = CreateSut().WithEnableInheritance(false).WithValidateArguments(input).Build();

            // Act
            var result = sut.AddValidationCode();

            // Assert
            result.Should().Be(input);
        }

        [Fact]
        public void Returns_None_When_EnableInheritance_Is_True_And_IsAbstract_Is_Also_True()
        {
            // Arrange
            var sut = CreateSut()
                .WithEnableInheritance()
                .WithIsAbstract()
                .WithValidateArguments(ArgumentValidationType.IValidatableObject)
                .Build();

            // Act
            var result = sut.AddValidationCode();

            // Assert
            result.Should().Be(ArgumentValidationType.None);
        }

        [Fact]
        public void Returns_None_When_EnableInheritance_Is_True_But_IsAbstract_Is_False_Without_BaseClass()
        {
            // Arrange
            var sut = CreateSut()
                .WithEnableInheritance()
                .WithIsAbstract(false)
                .WithBaseClass(null)
                .WithValidateArguments(ArgumentValidationType.IValidatableObject)
                .Build();

            // Act
            var result = sut.AddValidationCode();

            // Assert
            result.Should().Be(ArgumentValidationType.None);
        }

        [Theory]
        [InlineData(ArgumentValidationType.IValidatableObject)]
        [InlineData(ArgumentValidationType.None)]
        public void Returns_ValidateArguments_When_EnableInheritance_Is_True_But_IsAbstract_Is_False_With_BaseClass(ArgumentValidationType input)
        {
            // Arrange
            var sut = CreateSut()
                .WithEnableInheritance()
                .WithIsAbstract(false)
                .WithBaseClass(new ClassBuilder().WithName("MyBaseClass"))
                .WithValidateArguments(input)
                .Build();

            // Act
            var result = sut.AddValidationCode();

            // Assert
            result.Should().Be(input);
        }
    }
}

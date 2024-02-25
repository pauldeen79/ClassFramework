namespace ClassFramework.Domain.Tests.Builders;

public class PropertyBuilderTests : TestBase<PropertyBuilder>
{
    public class Constructor : PropertyBuilderTests
    {
        [Fact]
        public void Sets_HasGetter_To_True()
        {
            // Act
            var sut = CreateSut();

            // Assert
            sut.HasGetter.Should().BeTrue();
        }

        [Fact]
        public void Sets_HasSetter_To_True()
        {
            // Act
            var sut = CreateSut();

            // Assert
            sut.HasSetter.Should().BeTrue();
        }
    }

    public class Instance : PropertyBuilderTests
    {
        // added to prove code generation is configured correctly.
        // we want to be able to validate builders before building, using default System.ComponentModel.DataAnnotations.Validator functionality
        // this can either be done using shared validation (which allows custom code using the IValidatableObject interface), or domain only validation with copied (validation) attributes (which does not allow custom validation code)
        [Fact]
        public void Can_Be_Validated_Using_Standard_Dotnet_Validation()
        {
            // Arrange
            var sut = CreateSut().WithName(string.Empty);
            var validationResults = new List<ValidationResult>();

            // Act
            var result = Validator.TryValidateObject(sut, new ValidationContext(sut), validationResults);

            // Assert
            result.Should().BeFalse();
            validationResults.Should().NotBeEmpty();
        }
    }
}

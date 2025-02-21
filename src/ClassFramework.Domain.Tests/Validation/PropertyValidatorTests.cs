namespace ClassFramework.Domain.Tests.Validation;

public class PropertyValidatorTests
{
    [CustomValidation(typeof(PropertyValidator), nameof(PropertyValidator.Validate))] // test: illegal placement, only works on Property or PropertyBuilder, obviously!
    public class Validate
    {
        [Fact]
        public void Returns_Success_When_Instance_Is_Null()
        {
            // Arrange
            object instance = null!;

            // Act
            var result = PropertyValidator.Validate(instance);

            // Assert
            result.ShouldBeEquivalentTo(ValidationResult.Success);
        }

        [Fact]
        public void Returns_Failure_When_Applied_To_Instance_That_It_Not_View_Or_ViewBuilder()
        {
            // Arrange
            object instance = this; //wrong type!

            // Act
            var result = PropertyValidator.Validate(instance);

            // Assert
            result.ErrorMessage.ShouldBe("The PropertyValidator attribute can only be applied to Property and PropertyBuilder types");
        }

        [Fact]
        public void Returns_Failure_When_Instance_Has_Both_Setter_And_Initializer()
        {
            // Arrange
            object instance = new PropertyBuilder().WithName("MyProperty").WithType(typeof(int)).WithHasSetter().WithHasInitializer();

            // Act
            var result = PropertyValidator.Validate(instance);

            // Assert
            result.ErrorMessage.ShouldBe("HasSetter and HasInitializer cannot both be true");
        }


        [Fact]
        public void Returns_Success_When_Instance_Only_Has_Setter()
        {
            // Arrange
            object instance = new PropertyBuilder().WithName("MyProperty").WithType(typeof(int)).WithHasSetter().WithHasInitializer(false);

            // Act
            var result = PropertyValidator.Validate(instance);

            // Assert
            result.ShouldBeEquivalentTo(ValidationResult.Success);
        }

        [Fact]
        public void Returns_Success_When_Instance_Only_Has_Initializer()
        {
            // Arrange
            object instance = new PropertyBuilder().WithName("MyProperty").WithType(typeof(int)).WithHasSetter(false).WithHasInitializer();

            // Act
            var result = PropertyValidator.Validate(instance);

            // Assert
            result.ShouldBeEquivalentTo(ValidationResult.Success);
        }
    }
}

namespace ClassFramework.Domain.Tests.Builders.Types;

public class ClassBuilderTests : TestBase<ClassBuilder>
{
    [Fact]
    public void Can_Validate_Recursively_Using_TryValidate()
    {
        // Arrange
        var sut = CreateSut().AddFields(new FieldBuilder());
        var validationResults = new List<ValidationResult>();

        // Act
        var success = sut.TryValidate(validationResults);

        // Assert
        success.Should().BeFalse();
        validationResults.Should().HaveCount(2); //both the validation errors in Class and Field
    }

    [Fact]
    public void Can_Validate_Recursively_Using_Validate()
    {
        // Arrange
        var sut = CreateSut().AddFields(new FieldBuilder());
        var validationContext = new ValidationContext(sut);
        var validatableObject = sut as IValidatableObject; // can only do this with shared validation... else, the builder would not implement IValidatableObject by default

        // Act & Assert
        if (validatableObject is not null)
        {
            validatableObject.Invoking(x => x.Validate(validationContext)).Should().Throw<ValidationException>();
        }
    }
}

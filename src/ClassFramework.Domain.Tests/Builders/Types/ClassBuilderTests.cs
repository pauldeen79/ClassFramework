namespace ClassFramework.Domain.Tests.Builders.Types;

public class ClassBuilderTests : TestBase<ClassBuilder>
{
    [Fact]
    public void Can_Validate_Recursively()
    {
        // Arrange
        var sut = CreateSut().AddFields(new FieldBuilder());

        // Act
        var validationResults = new List<ValidationResult>();
        var success = sut.TryValidate(validationResults);

        // Assert
        success.Should().BeFalse();
        validationResults.Should().HaveCount(2); //both the validation errors in Class and Field
    }
}

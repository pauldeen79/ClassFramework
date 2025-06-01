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
        success.ShouldBeFalse();
        validationResults.Count.ShouldBe(2); ///both the validation errors in Class and Field;
    }
}

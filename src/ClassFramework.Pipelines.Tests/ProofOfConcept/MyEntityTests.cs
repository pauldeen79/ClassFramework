namespace ProofOfConcept;

public class MyEntityTests
{
    [Fact]
    public void Can_Construct_Entity_Using_Builder()
    {
        // Arrange
        var builder = new MyEntityBuilder();

        // Act
        var originalId = builder.Id();
        var originalName = builder.Name();
        var entity = builder
            .Id(1)
            .Name("John Doe")
            .Build();

        // Assert
        originalId.ShouldBe(0);
        originalName.ShouldBeNull();
        entity.Id.ShouldBe(1);
        entity.Name.ShouldBe("John Doe");
    }

    [Fact]
    public void Can_Not_Validate_Builder()
    {
        // Arrange
        var sut = new MyEntityBuilder();

        // Act
        var validationResults = new List<ValidationResult>();
        var validationResult = sut.TryValidate(validationResults);

        // Assert
        validationResult.ShouldBeTrue(); // We're expecting false because the name is required... But we can't see because validation only works with properties.
        validationResults.Count.ShouldBe(0);
    }
}
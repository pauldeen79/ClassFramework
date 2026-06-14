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
}
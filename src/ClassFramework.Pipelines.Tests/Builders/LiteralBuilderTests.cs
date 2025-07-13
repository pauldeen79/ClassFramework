namespace ClassFramework.Pipelines.Tests.Builders;

public class LiteralBuilderTests
{
    [Fact]
    public void Can_Construct_Without_OriginalValue()
    {
        // Act
        var sut = new LiteralBuilder("Name");

        // Assert
        sut.Value.ShouldBe("Name");
        sut.OriginalValue.ShouldBeNull();
    }
}

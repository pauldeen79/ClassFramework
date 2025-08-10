namespace ClassFramework.Domain.Tests.Builders;

public class LiteralBuilderTests : TestBase<LiteralBuilder>
{
    [Fact]
    public void Can_Cast_Builder_To_Entity()
    {
        // Arrange
        Literal entity = new LiteralBuilder("value");

        // Act
        LiteralBuilder builder = entity.ToBuilder();

        // Assert
        builder.Value.ShouldBe(entity.Value);
    }
}

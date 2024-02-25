namespace ClassFramework.Domain.Tests;

// note that this class is now actually generated code.
// but let's keep these tests, as a kind of integration test of the code generation process
public class LiteralTests
{
    [Fact]
    public void Constructs_Correclty_With_OriginalValue()
    {
        // Act
        var sut = new Literal("value", "original value");

        // Assert
        sut.Value.Should().Be("value");
        sut.OriginalValue.Should().Be("original value");
    }

    [Fact]
    public void Constructs_Correclty_Without_OriginalValue()
    {
        // Act
        var sut = new Literal("value", null);

        // Assert
        sut.Value.Should().Be("value");
        sut.OriginalValue.Should().BeNull();
    }
}

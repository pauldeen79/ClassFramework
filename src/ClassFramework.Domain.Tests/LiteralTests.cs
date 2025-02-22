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
        sut.Value.ShouldBe("value");
        sut.OriginalValue.ShouldBe("original value");
    }

    [Fact]
    public void Constructs_Correclty_Without_OriginalValue()
    {
        // Act
        var sut = new Literal("value", null);

        // Assert
        sut.Value.ShouldBe("value");
        sut.OriginalValue.ShouldBeNull();
    }
}

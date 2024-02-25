namespace ClassFramework.Pipelines.Tests.Extensions;

public class FormatProviderExtensionsTests : TestBase
{
    [Fact]
    public void ToCultureInfo_Returns_Instance_When_Input_Is_CultureInfo()
    {
        // Arrange
        IFormatProvider input = CultureInfo.CurrentCulture;

        // Act
        var result = input.ToCultureInfo();

        // Assert
        result.Should().BeSameAs(input);
    }

    [Fact]
    public void ToCultureInfo_Returns_InvariantCulture_When_Input_Is_Not_CultureInfo()
    {
        // Arrange
        IFormatProvider input = Fixture.Freeze<IFormatProvider>();

        // Act
        var result = input.ToCultureInfo();

        // Assert
        result.Name.Should().Be(CultureInfo.CurrentCulture.Name);
    }
}

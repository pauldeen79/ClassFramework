namespace ClassFramework.Pipelines.Tests.Extensions;

public class EnumerableOfMetadataExtensionsTests
{
    [Fact]
    public void CanGetValueWhenPresent()
    {
        // Arrange
        var lst = new[] { new Metadata("value", "name") };

        // Act
        var actual = lst.GetStringValue("name", "default");

        // Assert
        actual.Should().Be("value");
    }

    [Fact]
    public void CanGetDefaultValueWhenNotPresent()
    {
        // Arrange
        var lst = new[] { new Metadata("value", "other name") };

        // Act
        var actual = lst.GetStringValue("name", "default");

        // Assert
        actual.Should().Be("default");
    }

    [Fact]
    public void GetsFirstValueWhenPresent()
    {
        // Arrange
        var lst = new[] { new Metadata("value", "name"), new Metadata("second value", "name") };

        // Act
        var actual = lst.GetStringValue("name", "default");

        // Assert
        actual.Should().Be("value");
    }

    [Fact]
    public void CanGetMultipleValues()
    {
        // Arrange
        var lst = new[] { new Metadata("value", "name"), new Metadata("second value", "name") };

        // Act
        var actual = lst.GetStringValues("name");

        // Assert
        actual.Should().BeEquivalentTo("value", "second value");
    }

    [Fact]
    public void CanGetBooleanValue()
    {
        // Arrange
        var lst = new[] { new Metadata(true, "name"), new Metadata(false, "name") };

        // Act
        var actual = lst.GetBooleanValue("name");

        // Assert
        actual.Should().BeTrue();
    }

    [Fact]
    public void GetBooleanValueWithDefaultValueReturnsDefaultWhenNotFound()
    {
        // Arrange
        var lst = new[] { new Metadata(true, "name"), new Metadata(false, "name") };

        // Act
        var actual = lst.GetBooleanValue("wrongname", true);

        // Assert
        actual.Should().BeTrue();
    }

    [Fact]
    public void GetBooleanValueWithDefaultValueDelegateReturnsDefaultWhenNotFound()
    {
        // Arrange
        var lst = new[] { new Metadata(true, "name"), new Metadata(false, "name") };

        // Act
        var actual = lst.GetBooleanValue("name", () => true);

        // Assert
        actual.Should().BeTrue();
    }

    [Fact]
    public void CanGetEnumValueWhenPresent()
    {
        // Arrange
        var sut = new[] { new Metadata($"{MyEnumThing.A}", "Test") };

        // Act
        var actual = sut.GetValue("Test", () => MyEnumThing.B);

        // Assert
        actual.Should().Be(MyEnumThing.A);
    }

    [Fact]
    public void CanGetDefaultValueFromEnumWhenNotPresent()
    {
        // Arrange
        var sut = new[] { new Metadata($"{MyEnumThing.A}", "Test") };

        // Act
        var actual = sut.GetValue("WrongName", () => MyEnumThing.B);

        // Assert
        actual.Should().Be(MyEnumThing.B);
    }

    [Fact]
    public void CanGetNullableEnumValueWhenPresent()
    {
        // Arrange
        var sut = new[] { new Metadata($"{MyEnumThing.A}", "Test") };

        // Act
        var actual = sut.GetValue<MyEnumThing?>("Test", () => MyEnumThing.B);

        // Assert
        actual.Should().Be(MyEnumThing.A);
    }

    [Fact]
    public void CanGetDefaultValueFromNullableEnumWhenNotPresent()
    {
        // Arrange
        var sut = new[] { new Metadata($"{MyEnumThing.A}", "Test") };

        // Act
        var actual = sut.GetValue<MyEnumThing?>("WrongName", () => MyEnumThing.B);

        // Assert
        actual.Should().Be(MyEnumThing.B);
    }

    [Fact]
    public void CanGetValueFromDifferentType()
    {
        // Arrange
        var sut = new[] { new Metadata("1", "Test") };

        // Act
        var actual = sut.GetValue("Test", () => 0);

        // Assert
        actual.Should().Be(1);
    }
}

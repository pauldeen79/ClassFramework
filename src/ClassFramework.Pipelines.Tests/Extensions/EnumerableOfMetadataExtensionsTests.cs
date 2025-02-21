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
        actual.ShouldBe("value");
    }

    [Fact]
    public void CanGetDefaultValueWhenNotPresent()
    {
        // Arrange
        var lst = new[] { new Metadata("value", "other name") };

        // Act
        var actual = lst.GetStringValue("name", "default");

        // Assert
        actual.ShouldBe("default");
    }

    [Fact]
    public void GetsLastValueWhenPresent()
    {
        // Arrange
        var lst = new[] { new Metadata("value", "name"), new Metadata("second value", "name") };

        // Act
        var actual = lst.GetStringValue("name", "default");

        // Assert
        actual.ShouldBe("second value");
    }

    [Fact]
    public void CanGetMultipleValues()
    {
        // Arrange
        var lst = new[] { new Metadata("value", "name"), new Metadata("second value", "name") };

        // Act
        var actual = lst.GetStringValues("name");

        // Assert
        actual.ShouldBeEquivalentTo("value", "second value");
    }

    [Fact]
    public void CanGetBooleanValue()
    {
        // Arrange
        var lst = new[] { new Metadata(true, "name"), new Metadata(false, "name") };

        // Act
        var actual = lst.GetBooleanValue("name");

        // Assert
        actual.ShouldBeFalse();
    }

    [Fact]
    public void GetBooleanValueWithDefaultValueReturnsDefaultWhenNotFound()
    {
        // Arrange
        var lst = new[] { new Metadata(true, "name"), new Metadata(false, "name") };

        // Act
        var actual = lst.GetBooleanValue("wrongname", true);

        // Assert
        actual.ShouldBeTrue();
    }

    [Fact]
    public void GetBooleanValueWithDefaultValueDelegateReturnsDefaultWhenNotFound()
    {
        // Arrange
        var lst = new[] { new Metadata(true, "name"), new Metadata(false, "name") };

        // Act
        var actual = lst.GetBooleanValue("wrong name", () => true);

        // Assert
        actual.ShouldBeTrue();
    }

    [Fact]
    public void CanGetEnumValueWhenPresent()
    {
        // Arrange
        var sut = new[] { new Metadata($"{MyEnumThing.A}", "Test") };

        // Act
        var actual = sut.GetValue("Test", () => MyEnumThing.B);

        // Assert
        actual.ShouldBe(MyEnumThing.A);
    }

    [Fact]
    public void CanGetDefaultValueFromEnumWhenNotPresent()
    {
        // Arrange
        var sut = new[] { new Metadata($"{MyEnumThing.A}", "Test") };

        // Act
        var actual = sut.GetValue("WrongName", () => MyEnumThing.B);

        // Assert
        actual.ShouldBe(MyEnumThing.B);
    }

    [Fact]
    public void CanGetNullableEnumValueWhenPresent()
    {
        // Arrange
        var sut = new[] { new Metadata($"{MyEnumThing.A}", "Test") };

        // Act
        var actual = sut.GetValue<MyEnumThing?>("Test", () => MyEnumThing.B);

        // Assert
        actual.ShouldBe(MyEnumThing.A);
    }

    [Fact]
    public void CanGetDefaultValueFromNullableEnumWhenNotPresent()
    {
        // Arrange
        var sut = new[] { new Metadata($"{MyEnumThing.A}", "Test") };

        // Act
        var actual = sut.GetValue<MyEnumThing?>("WrongName", () => MyEnumThing.B);

        // Assert
        actual.ShouldBe(MyEnumThing.B);
    }

    [Fact]
    public void CanGetValueFromDifferentType()
    {
        // Arrange
        var sut = new[] { new Metadata("1", "Test") };

        // Act
        var actual = sut.GetValue("Test", () => 0);

        // Assert
        actual.ShouldBe(1);
    }
}

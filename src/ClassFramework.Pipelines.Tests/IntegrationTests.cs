namespace ClassFramework.Pipelines.Tests;

public class IntegrationTests : IntegrationTestBase<IFormattableStringParser>
{
    [Fact]
    public void Can_Use_FormattableStringParser_With_PlaceholderProcessors_To_Get_Formatted_String()
    {
        // Arrange
        var formatString = "foreach (var item in {NameCamelCsharpFriendlyName}) {$property.Name}.Add(item);";
        var sut = CreateSut();
        var property = new PropertyBuilder().WithName("MyProperty").WithType(typeof(string)).Build();
        var pipelineSettings = new PipelineSettingsBuilder().Build();
        var propertyContext = new PropertyContext(property, pipelineSettings, CultureInfo.InvariantCulture, "MyTypeName", typeof(List<string>).FullName!.WithoutProcessedGenerics());

        // Act
        var result = sut.Parse(formatString, CultureInfo.InvariantCulture, propertyContext);

        // Assert
        result.Status.Should().Be(ResultStatus.Ok);
        result.GetValueOrThrow().ToString().Should().Be("foreach (var item in myProperty) MyProperty.Add(item);");
    }

    [Fact]
    public void Can_Use_FormattableStringParser_With_Functions_To_Get_Formatted_String_And_Apply_Processing_Like_Casing()
    {
        // Arrange
        var formatString = "foreach (var item in {ToCamelCase(PropertyName())}) {PropertyName()}.Add(item);";
        var sut = CreateSut();
        var property = new PropertyBuilder().WithName("MyProperty").WithType(typeof(string)).Build();
        var pipelineSettings = new PipelineSettingsBuilder().Build();
        var propertyContext = new PropertyContext(property, pipelineSettings, CultureInfo.InvariantCulture, "MyTypeName", typeof(List<string>).FullName!.WithoutProcessedGenerics());

        // Act
        var result = sut.Parse(formatString, CultureInfo.InvariantCulture, propertyContext);

        // Assert
        result.Status.Should().Be(ResultStatus.Ok);
        result.GetValueOrThrow().ToString().Should().Be("foreach (var item in myProperty) MyProperty.Add(item);");
    }

    [Fact]
    public void Can_Use_FormattableStringParser_With_Functions_And_Variables_To_Get_Formatted_String_And_Apply_Processing_Like_Casing()
    {
        // Arrange
        var formatString = "foreach (var item in {ToCamelCase($property.Name)}) {$property.Name}.Add(item);";
        var sut = CreateSut();
        var property = new PropertyBuilder().WithName("MyProperty").WithType(typeof(string)).Build();
        var pipelineSettings = new PipelineSettingsBuilder().Build();
        var propertyContext = new PropertyContext(property, pipelineSettings, CultureInfo.InvariantCulture, "MyTypeName", typeof(List<string>).FullName!.WithoutProcessedGenerics());

        // Act
        var result = sut.Parse(formatString, CultureInfo.InvariantCulture, propertyContext);

        // Assert
        result.Status.Should().Be(ResultStatus.Ok);
        result.GetValueOrThrow().ToString().Should().Be("foreach (var item in myProperty) MyProperty.Add(item);");
    }
}

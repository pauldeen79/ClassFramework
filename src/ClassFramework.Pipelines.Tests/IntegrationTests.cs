namespace ClassFramework.Pipelines.Tests;

public class IntegrationTests : IntegrationTestBase<IFormattableStringParser>
{
    [Fact]
    public void Can_Use_FormattableStringParser_With_PlaceholderProcessors_To_Get_Formatted_String()
    {
        // Arrange
        var formatString = "foreach (var item in {CsharpFriendlyName(ToCamelCase($property.Name))}) {$property.Name}.Add(item);";
        var sut = CreateSut();
        var property = new PropertyBuilder().WithName("MyProperty").WithType(typeof(string)).Build();
        var pipelineSettings = new PipelineSettingsBuilder();
        var propertyContext = new PropertyContext(property, pipelineSettings, CultureInfo.InvariantCulture, "MyTypeName", typeof(List<string>).FullName!.WithoutGenerics());

        // Act
        var result = sut.Parse(formatString, CultureInfo.InvariantCulture, propertyContext);

        // Assert
        result.Status.ShouldBe(ResultStatus.Ok);
        result.GetValueOrThrow().ToString().ShouldBe("foreach (var item in myProperty) MyProperty.Add(item);");
    }

    [Fact]
    public void Can_Use_FormattableStringParser_With_Functions_To_Get_Formatted_String_And_Apply_Processing_Like_Casing()
    {
        // Arrange
        var formatString = "foreach (var item in {ToCamelCase($property.Name)}) {$property.Name}.Add(item);";
        var sut = CreateSut();
        var property = new PropertyBuilder().WithName("MyProperty").WithType(typeof(string)).Build();
        var pipelineSettings = new PipelineSettingsBuilder();
        var propertyContext = new PropertyContext(property, pipelineSettings, CultureInfo.InvariantCulture, "MyTypeName", typeof(List<string>).FullName!.WithoutGenerics());

        // Act
        var result = sut.Parse(formatString, CultureInfo.InvariantCulture, propertyContext);

        // Assert
        result.Status.ShouldBe(ResultStatus.Ok);
        result.GetValueOrThrow().ToString().ShouldBe("foreach (var item in myProperty) MyProperty.Add(item);");
    }

    [Fact]
    public void Can_Get_Descriptors_For_All_Functions()
    {
        // Arrange
        var descriptorProvider = Scope!.ServiceProvider.GetRequiredService<IFunctionDescriptorProvider>();

        // Act
        var result = descriptorProvider.GetAll();

        // Assert
        result.Count.ShouldBeGreaterThan(0);
    }
}

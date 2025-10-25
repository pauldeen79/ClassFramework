namespace ClassFramework.Pipelines.Tests;

public class IntegrationTests : IntegrationTestBase<IExpressionEvaluator>
{
    [Fact]
    public async Task Can_Use_ExpressionEvaluator_With_PlaceholderProcessors_To_Get_Formatted_String()
    {
        // Arrange
        var formatString = "foreach (var item in {CsharpFriendlyName(property.Name.ToCamelCase())}) {property.Name}.Add(item);";
        var sut = CreateSut();
        var property = new PropertyBuilder().WithName("MyProperty").WithType(typeof(string)).Build();
        var pipelineSettings = new PipelineSettingsBuilder();
        var propertyContext = new PropertyContext(property, pipelineSettings, CultureInfo.InvariantCulture, "MyTypeName", typeof(List<string>).FullName!.WithoutGenerics(), CancellationToken.None);

        // Act
        var result = await sut.EvaluateInterpolatedStringAsync(formatString, CultureInfo.InvariantCulture, propertyContext, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.Ok);
        result.GetValueOrThrow().ToString().ShouldBe("foreach (var item in myProperty) MyProperty.Add(item);");
    }

    [Fact]
    public async Task Can_Use_ExpressionEvaluator_With_Functions_To_Get_Formatted_String_And_Apply_Processing_Like_Casing()
    {
        // Arrange
        var formatString = "foreach (var item in {property.Name.ToCamelCase()}) {property.Name}.Add(item);";
        var sut = CreateSut();
        var property = new PropertyBuilder().WithName("MyProperty").WithType(typeof(string)).Build();
        var pipelineSettings = new PipelineSettingsBuilder();
        var propertyContext = new PropertyContext(property, pipelineSettings, CultureInfo.InvariantCulture, "MyTypeName", typeof(List<string>).FullName!.WithoutGenerics(), CancellationToken.None);

        // Act
        var result = await sut.EvaluateInterpolatedStringAsync(formatString, CultureInfo.InvariantCulture, propertyContext, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.Ok);
        result.GetValueOrThrow().ToString().ShouldBe("foreach (var item in myProperty) MyProperty.Add(item);");
    }

    [Fact]
    public void Can_Get_Descriptors_For_All_Members()
    {
        // Arrange
        var descriptorProvider = Scope!.ServiceProvider.GetRequiredService<IMemberDescriptorProvider>();

        // Act
        var result = descriptorProvider.GetAll();

        // Assert
        result.Status.ShouldBe(ResultStatus.Ok);
        result.Value.ShouldNotBeNull();
        result.Value.Count.ShouldBeGreaterThan(0);
    }
}

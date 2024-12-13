namespace ClassFramework.Pipelines.Tests.Shared.Variables;

public class NullCheckVariableTests : TestBase<NullCheckVariable>
{
    [Fact]
    public void Can_Get_PropertyName_From_ContextBase()
    {
        // Arrange
        var context = new PropertyContext(CreateProperty(), new PipelineSettingsBuilder().WithAddNullChecks().Build(), CultureInfo.InvariantCulture, typeof(string).FullName!, typeof(List<>).WithoutGenerics());
        var sut = CreateSut();

        // Act
        var result = sut.Process("nullCheck", context);

        // Assert
        result.Status.Should().Be(ResultStatus.Ok);
        result.Value.Should().Be("is null");
    }

    [Fact]
    public void Supplying_Unknown_Context_Type_Gives_Invalid_Result()
    {
        // Arrange
        var context = new object();
        var sut = CreateSut();

        // Act
        var result = sut.Process("nullCheck", context);

        // Assert
        result.Status.Should().Be(ResultStatus.Invalid);
        result.ErrorMessage.Should().Be("Could not get null check from context, because the context type System.Object is not supported");
    }

    [Fact]
    public void Supplying_Null_Context_Gives_Invalid_Result()
    {
        // Arrange
        var context = default(object?);
        var sut = CreateSut();

        // Act
        var result = sut.Process("nullCheck", context);

        // Assert
        result.Status.Should().Be(ResultStatus.Invalid);
        result.ErrorMessage.Should().Be("Could not get null check from context, because the context type null is not supported");
    }
}

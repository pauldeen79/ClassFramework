namespace ClassFramework.Pipelines.Tests.Shared.Variables;

public class ClassVariableTests : TestBase<ClassVariable>
{
    [Fact]
    public void Can_Get_ClassName_From_PropertyContext()
    {
        // Arrange
        var context = new PipelineContext<BuilderContext>(new BuilderContext(CreateClass(), new PipelineSettingsBuilder().Build(), CultureInfo.InvariantCulture));
        var sut = CreateSut();

        // Act
        var result = sut.Process("class.Name", context);

        // Assert
        result.Status.Should().Be(ResultStatus.Ok);
        result.Value.Should().Be(context.Request.SourceModel.Name);
    }

    [Fact]
    public void Supplying_Unknown_Context_Type_Gives_Invalid_Result()
    {
        // Arrange
        var context = new object();
        var sut = CreateSut();

        // Act
        var result = sut.Process("class.Name", context);

        // Assert
        result.Status.Should().Be(ResultStatus.Invalid);
        result.ErrorMessage.Should().Be("Could not get class from context, because the context type System.Object is not supported");
    }

    [Fact]
    public void Supplying_Null_Context_Gives_Invalid_Result()
    {
        // Arrange
        var context = default(object?);
        var sut = CreateSut();

        // Act
        var result = sut.Process("class.Name", context);

        // Assert
        result.Status.Should().Be(ResultStatus.Invalid);
        result.ErrorMessage.Should().Be("Could not get class from context, because the context type null is not supported");
    }

    [Fact]
    public void Supplying_Unknown_Property_Name_Gives_Continue_Result()
    {
        // Arrange
        var context = new PipelineContext<BuilderContext>(new BuilderContext(CreateClass(), new PipelineSettingsBuilder().Build(), CultureInfo.InvariantCulture));
        var sut = CreateSut();

        // Act
        var result = sut.Process("class.WrongPropertyName", context);

        // Assert
        result.Status.Should().Be(ResultStatus.Continue);
    }

    private static TypeBase CreateClass()
        => new ClassBuilder().WithName("MyClass").Build();
}

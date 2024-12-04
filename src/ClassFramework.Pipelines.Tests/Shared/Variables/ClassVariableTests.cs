namespace ClassFramework.Pipelines.Tests.Shared.Variables;

public class ClassVariableTests : TestBase<ClassVariable>
{
    [Fact]
    public void Can_Get_ClassName_From_BuilderContext()
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
    public void Can_Get_ClassName_From_BuilderExtensionContext()
    {
        // Arrange
        var context = new PipelineContext<BuilderExtensionContext>(new BuilderExtensionContext(CreateClass(), new PipelineSettingsBuilder().Build(), CultureInfo.InvariantCulture));
        var sut = CreateSut();

        // Act
        var result = sut.Process("class.Name", context);

        // Assert
        result.Status.Should().Be(ResultStatus.Ok);
        result.Value.Should().Be(context.Request.SourceModel.Name);
    }

    [Fact]
    public void Can_Get_ClassName_From_EntityContext()
    {
        // Arrange
        var context = new PipelineContext<EntityContext>(new EntityContext(CreateClass(), new PipelineSettingsBuilder().Build(), CultureInfo.InvariantCulture));
        var sut = CreateSut();

        // Act
        var result = sut.Process("class.Name", context);

        // Assert
        result.Status.Should().Be(ResultStatus.Ok);
        result.Value.Should().Be(context.Request.SourceModel.Name);
    }

    [Fact]
    public void Can_Get_ClassName_From_InterfaceContext()
    {
        // Arrange
        var context = new PipelineContext<InterfaceContext>(new InterfaceContext(CreateClass(), new PipelineSettingsBuilder().Build(), CultureInfo.InvariantCulture));
        var sut = CreateSut();

        // Act
        var result = sut.Process("class.Name", context);

        // Assert
        result.Status.Should().Be(ResultStatus.Ok);
        result.Value.Should().Be(context.Request.SourceModel.Name);
    }

    [Fact]
    public void Can_Get_ClassName_From_ReflectionContext()
    {
        // Arrange
        var context = new PipelineContext<ReflectionContext>(new ReflectionContext(GetType(), new PipelineSettingsBuilder().Build(), CultureInfo.InvariantCulture));
        var sut = CreateSut();

        // Act
        var result = sut.Process("class.Name", context);

        // Assert
        result.Status.Should().Be(ResultStatus.Ok);
        result.Value.Should().Be(GetType().Name);
    }

    [Fact]
    public void Can_Get_ClassName_From_ParentChildContext_Of_BuilderContext()
    {
        // Arrange
        var settings = new PipelineSettingsBuilder().Build();
        var context = new ParentChildContext<PipelineContext<BuilderContext>, Property>(new PipelineContext<BuilderContext>(new BuilderContext(new ClassBuilder().WithName("MyClass").Build(), settings, CultureInfo.InvariantCulture)), CreateProperty(), settings);
        var sut = CreateSut();

        // Act
        var result = sut.Process("class.Name", context);

        // Assert
        result.Status.Should().Be(ResultStatus.Ok);
        result.Value.Should().Be(context.ParentContext.Request.SourceModel.Name);
    }

    [Fact]
    public void Can_Get_ClassName_From_ParentChildContext_Of_BuilderExtensionContext()
    {
        // Arrange
        var settings = new PipelineSettingsBuilder().Build();
        var context = new ParentChildContext<PipelineContext<BuilderExtensionContext>, Property>(new PipelineContext<BuilderExtensionContext>(new BuilderExtensionContext(new ClassBuilder().WithName("MyClass").Build(), settings, CultureInfo.InvariantCulture)), CreateProperty(), settings);
        var sut = CreateSut();

        // Act
        var result = sut.Process("class.Name", context);

        // Assert
        result.Status.Should().Be(ResultStatus.Ok);
        result.Value.Should().Be(context.ParentContext.Request.SourceModel.Name);
    }

    [Fact]
    public void Can_Get_ClassFullName_From_BuilderContext()
    {
        // Arrange
        var context = new PipelineContext<BuilderContext>(new BuilderContext(CreateClass(), new PipelineSettingsBuilder().Build(), CultureInfo.InvariantCulture));
        var sut = CreateSut();

        // Act
        var result = sut.Process("class.FullName", context);

        // Assert
        result.Status.Should().Be(ResultStatus.Ok);
        result.Value.Should().Be(context.Request.SourceModel.GetFullName());
    }

    [Fact]
    public void Can_Get_ClassFullName_From_ReflectionContext()
    {
        // Arrange
        var context = new PipelineContext<ReflectionContext>(new ReflectionContext(GetType(), new PipelineSettingsBuilder().Build(), CultureInfo.InvariantCulture));
        var sut = CreateSut();

        // Act
        var result = sut.Process("class.FullName", context);

        // Assert
        result.Status.Should().Be(ResultStatus.Ok);
        result.Value.Should().Be(GetType().FullName);
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
}

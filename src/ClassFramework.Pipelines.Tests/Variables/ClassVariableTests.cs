namespace ClassFramework.Pipelines.Tests.Variables;

public class ClassVariableTests : TestBase<ClassVariable>
{
    [Fact]
    public void Can_Get_ClassName_From_BuilderContext()
    {
        // Arrange
        var context = new PipelineContext<BuilderContext>(new BuilderContext(CreateClass(), new PipelineSettingsBuilder().Build(), CultureInfo.InvariantCulture));
        var resolver = Fixture.Freeze<IObjectResolver>();
        resolver.Resolve<ClassModel>(Arg.Any<object?>()).Returns(Result.Success(new ClassModel(context.Request.SourceModel)));
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
        var resolver = Fixture.Freeze<IObjectResolver>();
        resolver.Resolve<ClassModel>(Arg.Any<object?>()).Returns(Result.Success(new ClassModel(context.Request.SourceModel)));
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
        var resolver = Fixture.Freeze<IObjectResolver>();
        resolver.Resolve<ClassModel>(Arg.Any<object?>()).Returns(Result.Success(new ClassModel(context.Request.SourceModel)));
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
        var resolver = Fixture.Freeze<IObjectResolver>();
        resolver.Resolve<ClassModel>(Arg.Any<object?>()).Returns(Result.Success(new ClassModel(context.Request.SourceModel)));
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
        var resolver = Fixture.Freeze<IObjectResolver>();
        resolver.Resolve<ClassModel>(Arg.Any<object?>()).Returns(Result.Success(new ClassModel(context.Request.SourceModel)));
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
        var resolver = Fixture.Freeze<IObjectResolver>();
        resolver.Resolve<ClassModel>(Arg.Any<object?>()).Returns(Result.Success(new ClassModel(context.ParentContext.Request.SourceModel)));
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
        var resolver = Fixture.Freeze<IObjectResolver>();
        resolver.Resolve<ClassModel>(Arg.Any<object?>()).Returns(Result.Success(new ClassModel(context.ParentContext.Request.SourceModel)));
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
        var resolver = Fixture.Freeze<IObjectResolver>();
        resolver.Resolve<ClassModel>(Arg.Any<object?>()).Returns(Result.Success(new ClassModel(context.Request.SourceModel)));
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
        var resolver = Fixture.Freeze<IObjectResolver>();
        resolver.Resolve<ClassModel>(Arg.Any<object?>()).Returns(Result.Success(new ClassModel(context.Request.SourceModel)));
        var sut = CreateSut();

        // Act
        var result = sut.Process("class.FullName", context);

        // Assert
        result.Status.Should().Be(ResultStatus.Ok);
        result.Value.Should().Be(GetType().FullName);
    }

    [Fact]
    public void Can_Get_ClassNamespace_From_BuilderContext()
    {
        // Arrange
        var context = new PipelineContext<BuilderContext>(new BuilderContext(CreateClass(), new PipelineSettingsBuilder().Build(), CultureInfo.InvariantCulture));
        var resolver = Fixture.Freeze<IObjectResolver>();
        resolver.Resolve<ClassModel>(Arg.Any<object?>()).Returns(Result.Success(new ClassModel(context.Request.SourceModel)));
        var sut = CreateSut();

        // Act
        var result = sut.Process("class.Namespace", context);

        // Assert
        result.Status.Should().Be(ResultStatus.Ok);
        result.Value.Should().Be(context.Request.SourceModel.Namespace);
    }

    [Fact]
    public void Returns_Non_Succesful_Result_From_ObjectResolver()
    {
        // Arrange
        var context = default(object?);
        var resolver = Fixture.Freeze<IObjectResolver>();
        resolver.Resolve<ClassModel>(Arg.Any<object?>()).Returns(Result.NotFound<ClassModel>());
        var sut = CreateSut();

        // Act
        var result = sut.Process("class.Name", context);

        // Assert
        result.Status.Should().Be(ResultStatus.NotFound);
    }

    [Fact]
    public void Supplying_Unknown_Property_Name_Gives_Continue_Result()
    {
        // Arrange
        var context = new PipelineContext<BuilderContext>(new BuilderContext(CreateClass(), new PipelineSettingsBuilder().Build(), CultureInfo.InvariantCulture));
        var resolver = Fixture.Freeze<IObjectResolver>();
        resolver.Resolve<ClassModel>(Arg.Any<object?>()).Returns(Result.Success(new ClassModel(context.Request.SourceModel)));
        var sut = CreateSut();

        // Act
        var result = sut.Process("class.WrongPropertyName", context);

        // Assert
        result.Status.Should().Be(ResultStatus.Continue);
    }
}

namespace ClassFramework.Pipelines.Tests.Variables;

public class PropertyVariableTests : TestBase<PropertyVariable>
{
    [Fact]
    public void Can_Get_PropertyName_From_PropertyContext()
    {
        // Arrange
        var context = new PropertyContext(CreateProperty(), new PipelineSettingsBuilder().Build(), CultureInfo.InvariantCulture, typeof(string).FullName!, typeof(List<>).WithoutGenerics());
        var resolver = Fixture.Freeze<IObjectResolver>();
        resolver.Resolve<Property>(Arg.Any<object?>()).Returns(Result.Success(context.SourceModel));
        resolver.Resolve<PipelineSettings>(Arg.Any<object?>()).Returns(Result.Success(context.Settings));
        resolver.Resolve<CultureInfo>(Arg.Any<object?>()).Returns(Result.Success(context.FormatProvider.ToCultureInfo()));
        resolver.Resolve<ITypeNameMapper>(Arg.Any<object?>()).Returns(Result.Success<ITypeNameMapper>(context));
        resolver.Resolve<MappedContextBase>(Arg.Any<object?>()).Returns(Result.Success<MappedContextBase>(context));
        var sut = CreateSut();

        // Act
        var result = sut.Process("property.Name", context);

        // Assert
        result.Status.Should().Be(ResultStatus.Ok);
        result.Value.Should().Be(context.SourceModel.Name);
    }

    [Fact]
    public void Can_Get_PropertyName_From_ParentChildContext_Of_BuilderContext()
    {
        // Arrange
        var settings = new PipelineSettingsBuilder().Build();
        var context = new ParentChildContext<PipelineContext<BuilderContext>, Property>(new PipelineContext<BuilderContext>(new BuilderContext(new ClassBuilder().WithName("MyClass").Build(), settings, CultureInfo.InvariantCulture)), CreateProperty(), settings);
        var resolver = Fixture.Freeze<IObjectResolver>();
        resolver.Resolve<Property>(Arg.Any<object?>()).Returns(Result.Success(context.ChildContext));
        resolver.Resolve<PipelineSettings>(Arg.Any<object?>()).Returns(Result.Success(context.Settings));
        resolver.Resolve<CultureInfo>(Arg.Any<object?>()).Returns(Result.Success(context.ParentContext.Request.FormatProvider.ToCultureInfo()));
        resolver.Resolve<ITypeNameMapper>(Arg.Any<object?>()).Returns(Result.Success<ITypeNameMapper>(context.ParentContext.Request));
        resolver.Resolve<MappedContextBase>(Arg.Any<object?>()).Returns(Result.Success<MappedContextBase>(context.ParentContext.Request));
        var sut = CreateSut();

        // Act
        var result = sut.Process("property.Name", context);

        // Assert
        result.Status.Should().Be(ResultStatus.Ok);
        result.Value.Should().Be(context.ChildContext.Name);
    }

    [Fact]
    public void Can_Get_PropertyName_From_ParentChildContext_Of_BuilderExtensionContext()
    {
        // Arrange
        var settings = new PipelineSettingsBuilder().Build();
        var context = new ParentChildContext<PipelineContext<BuilderExtensionContext>, Property>(new PipelineContext<BuilderExtensionContext>(new BuilderExtensionContext(new ClassBuilder().WithName("MyClass").Build(), settings, CultureInfo.InvariantCulture)), CreateProperty(), settings);
        var resolver = Fixture.Freeze<IObjectResolver>();
        resolver.Resolve<Property>(Arg.Any<object?>()).Returns(Result.Success(context.ChildContext));
        resolver.Resolve<PipelineSettings>(Arg.Any<object?>()).Returns(Result.Success(context.Settings));
        resolver.Resolve<CultureInfo>(Arg.Any<object?>()).Returns(Result.Success(context.ParentContext.Request.FormatProvider.ToCultureInfo()));
        resolver.Resolve<ITypeNameMapper>(Arg.Any<object?>()).Returns(Result.Success<ITypeNameMapper>(context.ParentContext.Request));
        resolver.Resolve<MappedContextBase>(Arg.Any<object?>()).Returns(Result.Success<MappedContextBase>(context.ParentContext.Request));
        var sut = CreateSut();

        // Act
        var result = sut.Process("property.Name", context);

        // Assert
        result.Status.Should().Be(ResultStatus.Ok);
        result.Value.Should().Be(context.ChildContext.Name);
    }

    [Fact]
    public void Can_Get_BuilderMemberName_From_PropertyContext()
    {
        // Arrange
        var context = new PropertyContext(CreateProperty(), new PipelineSettingsBuilder().Build(), CultureInfo.InvariantCulture, typeof(string).FullName!, typeof(List<>).WithoutGenerics());
        var resolver = Fixture.Freeze<IObjectResolver>();
        resolver.Resolve<Property>(Arg.Any<object?>()).Returns(Result.Success(context.SourceModel));
        resolver.Resolve<PipelineSettings>(Arg.Any<object?>()).Returns(Result.Success(context.Settings));
        resolver.Resolve<CultureInfo>(Arg.Any<object?>()).Returns(Result.Success(context.FormatProvider.ToCultureInfo()));
        resolver.Resolve<ITypeNameMapper>(Arg.Any<object?>()).Returns(Result.Success<ITypeNameMapper>(context));
        resolver.Resolve<MappedContextBase>(Arg.Any<object?>()).Returns(Result.Success<MappedContextBase>(context));
        var sut = CreateSut();

        // Act
        var result = sut.Process("property.BuilderMemberName", context);

        // Assert
        result.Status.Should().Be(ResultStatus.Ok);
        result.Value.Should().Be(context.SourceModel.Name);
    }

    [Fact]
    public void Can_Get_EntityMemberName_From_PropertyContext()
    {
        // Arrange
        var context = new PropertyContext(CreateProperty(), new PipelineSettingsBuilder().Build(), CultureInfo.InvariantCulture, typeof(string).FullName!, typeof(List<>).WithoutGenerics());
        var resolver = Fixture.Freeze<IObjectResolver>();
        resolver.Resolve<Property>(Arg.Any<object?>()).Returns(Result.Success(context.SourceModel));
        resolver.Resolve<PipelineSettings>(Arg.Any<object?>()).Returns(Result.Success(context.Settings));
        resolver.Resolve<CultureInfo>(Arg.Any<object?>()).Returns(Result.Success(context.FormatProvider.ToCultureInfo()));
        resolver.Resolve<ITypeNameMapper>(Arg.Any<object?>()).Returns(Result.Success<ITypeNameMapper>(context));
        resolver.Resolve<MappedContextBase>(Arg.Any<object?>()).Returns(Result.Success<MappedContextBase>(context));
        var sut = CreateSut();

        // Act
        var result = sut.Process("property.EntityMemberName", context);

        // Assert
        result.Status.Should().Be(ResultStatus.Ok);
        result.Value.Should().Be(context.SourceModel.Name);
    }

    [Fact]
    public void Can_Get_NullableRequiredSuffix_From_PropertyContext()
    {
        // Arrange
        var context = new PropertyContext(CreateProperty(), new PipelineSettingsBuilder().Build(), CultureInfo.InvariantCulture, typeof(string).FullName!, typeof(List<>).WithoutGenerics());
        var resolver = Fixture.Freeze<IObjectResolver>();
        resolver.Resolve<Property>(Arg.Any<object?>()).Returns(Result.Success(context.SourceModel));
        resolver.Resolve<PipelineSettings>(Arg.Any<object?>()).Returns(Result.Success(context.Settings));
        resolver.Resolve<CultureInfo>(Arg.Any<object?>()).Returns(Result.Success(context.FormatProvider.ToCultureInfo()));
        resolver.Resolve<ITypeNameMapper>(Arg.Any<object?>()).Returns(Result.Success<ITypeNameMapper>(context));
        resolver.Resolve<MappedContextBase>(Arg.Any<object?>()).Returns(Result.Success<MappedContextBase>(context));
        var sut = CreateSut();

        // Act
        var result = sut.Process("property.NullableRequiredSuffix", context);

        // Assert
        result.Status.Should().Be(ResultStatus.Ok);
        result.Value.Should().Be(string.Empty);
    }

    [Fact]
    public void Can_Get_PropertyTypeName_From_PropertyContext()
    {
        // Arrange
        var context = new PropertyContext(CreateProperty(), new PipelineSettingsBuilder().Build(), CultureInfo.InvariantCulture, typeof(string).FullName!, typeof(List<>).WithoutGenerics());
        var resolver = Fixture.Freeze<IObjectResolver>();
        resolver.Resolve<Property>(Arg.Any<object?>()).Returns(Result.Success(context.SourceModel));
        resolver.Resolve<PipelineSettings>(Arg.Any<object?>()).Returns(Result.Success(context.Settings));
        resolver.Resolve<CultureInfo>(Arg.Any<object?>()).Returns(Result.Success(context.FormatProvider.ToCultureInfo()));
        resolver.Resolve<ITypeNameMapper>(Arg.Any<object?>()).Returns(Result.Success<ITypeNameMapper>(context));
        resolver.Resolve<MappedContextBase>(Arg.Any<object?>()).Returns(Result.Success<MappedContextBase>(context));
        var sut = CreateSut();

        // Act
        var result = sut.Process("property.TypeName", context);

        // Assert
        result.Status.Should().Be(ResultStatus.Ok);
        result.Value.Should().Be(context.SourceModel.TypeName);
    }

    [Fact]
    public void Returns_Non_Succesful_Result_From_ObjectResolver()
    {
        // Arrange
        var context = default(object?);
        var resolver = Fixture.Freeze<IObjectResolver>();
        resolver.Resolve<Property>(Arg.Any<object?>()).Returns(Result.NotFound<Property>());
        var sut = CreateSut();

        // Act
        var result = sut.Process("property.Name", context);

        // Assert
        result.Status.Should().Be(ResultStatus.NotFound);
    }

    [Fact]
    public void Supplying_Unknown_Property_Name_Gives_Continue_Result()
    {
        // Arrange
        var context = new PropertyContext(CreateProperty(), new PipelineSettingsBuilder().Build(), CultureInfo.InvariantCulture, typeof(string).FullName!, typeof(List<>).WithoutGenerics());
        var resolver = Fixture.Freeze<IObjectResolver>();
        resolver.Resolve<Property>(Arg.Any<object?>()).Returns(Result.Success(context.SourceModel));
        resolver.Resolve<PipelineSettings>(Arg.Any<object?>()).Returns(Result.Success(context.Settings));
        resolver.Resolve<CultureInfo>(Arg.Any<object?>()).Returns(Result.Success(context.FormatProvider.ToCultureInfo()));
        resolver.Resolve<ITypeNameMapper>(Arg.Any<object?>()).Returns(Result.Success<ITypeNameMapper>(context));
        resolver.Resolve<MappedContextBase>(Arg.Any<object?>()).Returns(Result.Success<MappedContextBase>(context));
        var sut = CreateSut();

        // Act
        var result = sut.Process("property.WrongPropertyName", context);

        // Assert
        result.Status.Should().Be(ResultStatus.Continue);
    }
}

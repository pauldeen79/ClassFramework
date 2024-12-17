namespace ClassFramework.Pipelines.Tests.ObjectResolvers;

public class TypeNameMapperResolverTests : TestBase<TypeNameMapperResolver>
{
    public class Resolve : TypeNameMapperResolverTests
    {
        [Fact]
        public void Returns_Not_Supported_On_Unsupported_Source_Object()
        {
            // Arrange
            var sourceObject = new object();
            var sut = CreateSut();

            // Act
            var result = sut.Resolve<ITypeNameMapper>(sourceObject);

            // Assert
            result.Status.Should().Be(ResultStatus.NotSupported);
            result.ErrorMessage.Should().Be("Could not get typename mapper from context, because the context type System.Object is not supported");
        }

        [Fact]
        public void Returns_Not_Supported_On_Null_Source_Object()
        {
            // Arrange
            object? sourceObject = default;
            var sut = CreateSut();

            // Act
            var result = sut.Resolve<ITypeNameMapper>(sourceObject);

            // Assert
            result.Status.Should().Be(ResultStatus.NotSupported);
            result.ErrorMessage.Should().Be("Could not get typename mapper from context, because the context type null is not supported");
        }

        [Fact]
        public void Returns_Success_On_PropertyContext()
        {
            // Arrange
            var sourceObject = new PropertyContext(CreateProperty(), new PipelineSettingsBuilder().Build(), CultureInfo.InvariantCulture, typeof(string).FullName!, string.Empty);
            var sut = CreateSut();

            // Act
            var result = sut.Resolve<ITypeNameMapper>(sourceObject);

            // Assert
            result.Status.Should().Be(ResultStatus.Ok);
            result.Value.Should().NotBeNull();
        }

        [Fact]
        public void Can_Map_TypeName_On_PropertyContext()
        {
            // Arrange
            var sourceObject = new PropertyContext(CreateProperty(), new PipelineSettingsBuilder().Build(), CultureInfo.InvariantCulture, typeof(string).FullName!, string.Empty);
            var sut = CreateSut();
            var result = sut.Resolve<ITypeNameMapper>(sourceObject);

            // Act
            var typeName = result.GetValueOrThrow().MapTypeName(sourceObject.TypeName);

            // Assert
            typeName.Should().Be(sourceObject.TypeName);
        }

        [Fact]
        public void Returns_Success_On_ParentChildContext_Of_BuilderContext()
        {
            // Arrange
            var settings = new PipelineSettingsBuilder().Build();
            var sourceObject = new ParentChildContext<PipelineContext<BuilderContext>, Property>(new PipelineContext<BuilderContext>(new BuilderContext(new ClassBuilder().WithName("MyClass").Build(), settings, CultureInfo.InvariantCulture)), CreateProperty(), settings);
            var sut = CreateSut();

            // Act
            var result = sut.Resolve<ITypeNameMapper>(sourceObject);

            // Assert
            result.Status.Should().Be(ResultStatus.Ok);
            result.Value.Should().NotBeNull();
        }

        [Fact]
        public void Can_Map_TypeName_On_ParentChildContext_Of_BuilderContext()
        {
            // Arrange
            var settings = new PipelineSettingsBuilder().Build();
            var sourceObject = new ParentChildContext<PipelineContext<BuilderContext>, Property>(new PipelineContext<BuilderContext>(new BuilderContext(new ClassBuilder().WithName("MyClass").Build(), settings, CultureInfo.InvariantCulture)), CreateProperty(), settings);
            var sut = CreateSut();
            var result = sut.Resolve<ITypeNameMapper>(sourceObject);

            // Act
            var typeName = result.GetValueOrThrow().MapTypeName(sourceObject.ChildContext.TypeName);

            // Assert
            typeName.Should().Be(sourceObject.ChildContext.TypeName);
        }

        [Fact]
        public void Returns_Success_On_ParentChildContext_Of_BuilderExtensionContext()
        {
            // Arrange
            var settings = new PipelineSettingsBuilder().Build();
            var sourceObject = new ParentChildContext<PipelineContext<BuilderExtensionContext>, Property>(new PipelineContext<BuilderExtensionContext>(new BuilderExtensionContext(new ClassBuilder().WithName("MyClass").Build(), settings, CultureInfo.InvariantCulture)), CreateProperty(), settings);
            var sut = CreateSut();

            // Act
            var result = sut.Resolve<ITypeNameMapper>(sourceObject);

            // Assert
            result.Status.Should().Be(ResultStatus.Ok);
            result.Value.Should().NotBeNull();
        }

        [Fact]
        public void Can_Map_TypeName_On_ParentChildContext_Of_BuilderExtensionContext()
        {
            // Arrange
            var settings = new PipelineSettingsBuilder().Build();
            var sourceObject = new ParentChildContext<PipelineContext<BuilderExtensionContext>, Property>(new PipelineContext<BuilderExtensionContext>(new BuilderExtensionContext(new ClassBuilder().WithName("MyClass").Build(), settings, CultureInfo.InvariantCulture)), CreateProperty(), settings);
            var sut = CreateSut();
            var result = sut.Resolve<ITypeNameMapper>(sourceObject);

            // Act
            var typeName = result.GetValueOrThrow().MapTypeName(sourceObject.ChildContext.TypeName);

            // Assert
            typeName.Should().Be(sourceObject.ChildContext.TypeName);
        }

        [Fact]
        public void Returns_Continue_When_Type_Is_Not_TypeNameMapper()
        {
            // Arrange
            var sourceObject = new object();
            var sut = CreateSut();

            // Act
            var result = sut.Resolve<PropertyResolverTests>(sourceObject);

            // Assert
            result.Status.Should().Be(ResultStatus.Continue);
        }
    }
}

namespace ClassFramework.Pipelines.Tests.ObjectResolvers;

public class MappedContextBaseResolverTests : TestBase<MappedContextBaseResolver>
{
    public class Resolve : MappedContextBaseResolverTests
    {
        [Fact]
        public void Returns_Not_Supported_On_Unsupported_Source_Object()
        {
            // Arrange
            var sourceObject = new object();
            var sut = CreateSut();

            // Act
            var result = sut.Resolve<MappedContextBase>(sourceObject);

            // Assert
            result.Status.Should().Be(ResultStatus.NotSupported);
            result.ErrorMessage.Should().Be("Could not get MappedContextBase from context, because the context type System.Object is not supported");
        }

        [Fact]
        public void Returns_Not_Supported_On_Null_Source_Object()
        {
            // Arrange
            object? sourceObject = default;
            var sut = CreateSut();

            // Act
            var result = sut.Resolve<MappedContextBase>(sourceObject);

            // Assert
            result.Status.Should().Be(ResultStatus.NotSupported);
            result.ErrorMessage.Should().Be("Could not get MappedContextBase from context, because the context type null is not supported");
        }

        [Fact]
        public void Returns_Success_On_BuilderContext()
        {
            // Arrange
            var sourceObject = new BuilderContext(CreateClass(), new PipelineSettingsBuilder(), CultureInfo.InvariantCulture);
            var sut = CreateSut();

            // Act
            var result = sut.Resolve<MappedContextBase>(sourceObject);

            // Assert
            result.Status.Should().Be(ResultStatus.Ok);
            result.Value.Should().NotBeNull();
        }

        [Fact]
        public void Returns_Success_On_BuilderExtensionContext()
        {
            // Arrange
            var sourceObject = new BuilderExtensionContext(CreateClass(), new PipelineSettingsBuilder(), CultureInfo.InvariantCulture);
            var sut = CreateSut();

            // Act
            var result = sut.Resolve<MappedContextBase>(sourceObject);

            // Assert
            result.Status.Should().Be(ResultStatus.Ok);
            result.Value.Should().NotBeNull();
        }

        [Fact]
        public void Returns_Success_On_EntityContext()
        {
            // Arrange
            var sourceObject = new EntityContext(CreateClass(), new PipelineSettingsBuilder(), CultureInfo.InvariantCulture);
            var sut = CreateSut();

            // Act
            var result = sut.Resolve<MappedContextBase>(sourceObject);

            // Assert
            result.Status.Should().Be(ResultStatus.Ok);
            result.Value.Should().NotBeNull();
        }

        [Fact]
        public void Returns_Success_On_InterfaceContext()
        {
            // Arrange
            var sourceObject = new InterfaceContext(CreateClass(), new PipelineSettingsBuilder(), CultureInfo.InvariantCulture);
            var sut = CreateSut();

            // Act
            var result = sut.Resolve<MappedContextBase>(sourceObject);

            // Assert
            result.Status.Should().Be(ResultStatus.Ok);
            result.Value.Should().NotBeNull();
        }

        [Fact]
        public void Returns_Success_On_ReflectionContext()
        {
            // Arrange
            var sourceObject = new ReflectionContext(GetType(), new PipelineSettingsBuilder(), CultureInfo.InvariantCulture);
            var sut = CreateSut();

            // Act
            var result = sut.Resolve<MappedContextBase>(sourceObject);

            // Assert
            result.Status.Should().Be(ResultStatus.Ok);
            result.Value.Should().NotBeNull();
        }

        [Fact]
        public void Returns_Success_On_ParentChildContext_Of_BuilderContext()
        {
            // Arrange
            var settings = new PipelineSettingsBuilder();
            var sourceObject = new ParentChildContext<PipelineContext<BuilderContext>, Property>(new PipelineContext<BuilderContext>(new BuilderContext(new ClassBuilder().WithName("MyClass").Build(), settings, CultureInfo.InvariantCulture)), CreateProperty(), settings);
            var sut = CreateSut();

            // Act
            var result = sut.Resolve<MappedContextBase>(sourceObject);

            // Assert
            result.Status.Should().Be(ResultStatus.Ok);
            result.Value.Should().NotBeNull();
        }

        [Fact]
        public void Returns_Success_On_ParentChildContext_Of_BuilderExtensionContext()
        {
            // Arrange
            var settings = new PipelineSettingsBuilder();
            var sourceObject = new ParentChildContext<PipelineContext<BuilderExtensionContext>, Property>(new PipelineContext<BuilderExtensionContext>(new BuilderExtensionContext(new ClassBuilder().WithName("MyClass").Build(), settings, CultureInfo.InvariantCulture)), CreateProperty(), settings);
            var sut = CreateSut();

            // Act
            var result = sut.Resolve<MappedContextBase>(sourceObject);

            // Assert
            result.Status.Should().Be(ResultStatus.Ok);
            result.Value.Should().NotBeNull();
        }

        [Fact]
        public void Returns_Success_On_ParentChildContext_Of_EntityContext()
        {
            // Arrange
            var settings = new PipelineSettingsBuilder();
            var sourceObject = new ParentChildContext<PipelineContext<EntityContext>, Property>(new PipelineContext<EntityContext>(new EntityContext(new ClassBuilder().WithName("MyClass").Build(), settings, CultureInfo.InvariantCulture)), CreateProperty(), settings);
            var sut = CreateSut();

            // Act
            var result = sut.Resolve<MappedContextBase>(sourceObject);

            // Assert
            result.Status.Should().Be(ResultStatus.Ok);
            result.Value.Should().NotBeNull();
        }

        [Fact]
        public void Returns_Continue_When_Type_Is_Not_ClassModel()
        {
            // Arrange
            var sourceObject = new object();
            var sut = CreateSut();

            // Act
            var result = sut.Resolve<MappedContextBaseResolverTests>(sourceObject);

            // Assert
            result.Status.Should().Be(ResultStatus.Continue);
        }
    }
}

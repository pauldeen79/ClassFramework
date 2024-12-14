namespace ClassFramework.Pipelines.Tests.ObjectResolvers;

public class PropertyResolverTests : TestBase<PropertyResolver>
{
    public class Resolve : PropertyResolverTests
    {
        [Fact]
        public void Returns_Not_Supported_On_Unsupported_Context_Type()
        {
            // Arrange
            var sourceObject = new object();
            var sut = CreateSut();

            // Act
            var result = sut.Resolve<Property>(sourceObject);

            // Assert
            result.Status.Should().Be(ResultStatus.NotSupported);
            result.ErrorMessage.Should().Be("Could not get property from context, because the context type System.Object is not supported");
        }

        [Fact]
        public void Returns_Success_On_PropertyContext()
        {
            // Arrange
            var sourceObject = new PropertyContext(CreateProperty(), new PipelineSettingsBuilder().Build(), CultureInfo.InvariantCulture, typeof(string).FullName!, string.Empty); 
            var sut = CreateSut();

            // Act
            var result = sut.Resolve<Property>(sourceObject);

            // Assert
            result.Status.Should().Be(ResultStatus.Ok);
            result.Value.Should().NotBeNull();
        }

        [Fact]
        public void Returns_Success_On_ParentChildContext_Of_BuilderContext()
        {
            // Arrange
            var settings = new PipelineSettingsBuilder().Build();
            var sourceObject = new ParentChildContext<PipelineContext<BuilderContext>, Property>(new PipelineContext<BuilderContext>(new BuilderContext(new ClassBuilder().WithName("MyClass").Build(), settings, CultureInfo.InvariantCulture)), CreateProperty(), settings);
            var sut = CreateSut();

            // Act
            var result = sut.Resolve<Property>(sourceObject);

            // Assert
            result.Status.Should().Be(ResultStatus.Ok);
            result.Value.Should().NotBeNull();
        }

        [Fact]
        public void Returns_Success_On_ParentChildContext_Of_BuilderExtensionContext()
        {
            // Arrange
            var settings = new PipelineSettingsBuilder().Build();
            var sourceObject = new ParentChildContext<PipelineContext<BuilderExtensionContext>, Property>(new PipelineContext<BuilderExtensionContext>(new BuilderExtensionContext(new ClassBuilder().WithName("MyClass").Build(), settings, CultureInfo.InvariantCulture)), CreateProperty(), settings);
            var sut = CreateSut();

            // Act
            var result = sut.Resolve<Property>(sourceObject);

            // Assert
            result.Status.Should().Be(ResultStatus.Ok);
            result.Value.Should().NotBeNull();
        }

        [Fact]
        public void Returns_Success_On_ParentChildContext_Of_EntityContext()
        {
            // Arrange
            var settings = new PipelineSettingsBuilder().Build();
            var sourceObject = new ParentChildContext<PipelineContext<EntityContext>, Property>(new PipelineContext<EntityContext>(new EntityContext(new ClassBuilder().WithName("MyClass").Build(), settings, CultureInfo.InvariantCulture)), CreateProperty(), settings);
            var sut = CreateSut();

            // Act
            var result = sut.Resolve<Property>(sourceObject);

            // Assert
            result.Status.Should().Be(ResultStatus.Ok);
            result.Value.Should().NotBeNull();
        }

        [Fact]
        public void Returns_Continue_When_Type_Is_Not_Property()
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

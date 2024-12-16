﻿namespace ClassFramework.Pipelines.Tests.ObjectResolvers;

public class ClassModelResolverTests : TestBase<ClassModelResolver>
{
    public class Resolve : ClassModelResolverTests
    {
        [Fact]
        public void Returns_Not_Supported_On_Unsupported_Context_Type()
        {
            // Arrange
            var sourceObject = new object();
            var sut = CreateSut();

            // Act
            var result = sut.Resolve<ClassModel>(sourceObject);

            // Assert
            result.Status.Should().Be(ResultStatus.NotSupported);
            result.ErrorMessage.Should().Be("Could not get class from context, because the context type System.Object is not supported");
        }

        [Fact]
        public void Returns_Success_On_PipelineContext_Of_BuilderContext()
        {
            // Arrange
            var sourceObject = new PipelineContext<BuilderContext>(new BuilderContext(CreateClass(), new PipelineSettingsBuilder().Build(), CultureInfo.InvariantCulture));
            var sut = CreateSut();

            // Act
            var result = sut.Resolve<ClassModel>(sourceObject);

            // Assert
            result.Status.Should().Be(ResultStatus.Ok);
            result.Value.Should().NotBeNull();
        }

        [Fact]
        public void Returns_Success_On_PipelineContext_Of_BuilderExtensionContext()
        {
            // Arrange
            var sourceObject = new PipelineContext<BuilderExtensionContext>(new BuilderExtensionContext(CreateClass(), new PipelineSettingsBuilder().Build(), CultureInfo.InvariantCulture));
            var sut = CreateSut();

            // Act
            var result = sut.Resolve<ClassModel>(sourceObject);

            // Assert
            result.Status.Should().Be(ResultStatus.Ok);
            result.Value.Should().NotBeNull();
        }

        [Fact]
        public void Returns_Success_On_PipelineContext_Of_EntityContext()
        {
            // Arrange
            var sourceObject = new PipelineContext<EntityContext>(new EntityContext(CreateClass(), new PipelineSettingsBuilder().Build(), CultureInfo.InvariantCulture));
            var sut = CreateSut();

            // Act
            var result = sut.Resolve<ClassModel>(sourceObject);

            // Assert
            result.Status.Should().Be(ResultStatus.Ok);
            result.Value.Should().NotBeNull();
        }

        [Fact]
        public void Returns_Success_On_PipelineContext_Of_InterfaceContext()
        {
            // Arrange
            var sourceObject = new PipelineContext<InterfaceContext>(new InterfaceContext(CreateClass(), new PipelineSettingsBuilder().Build(), CultureInfo.InvariantCulture));
            var sut = CreateSut();

            // Act
            var result = sut.Resolve<ClassModel>(sourceObject);

            // Assert
            result.Status.Should().Be(ResultStatus.Ok);
            result.Value.Should().NotBeNull();
        }

        [Fact]
        public void Returns_Success_On_PipelineContext_Of_ReflectionContext()
        {
            // Arrange
            var sourceObject = new PipelineContext<ReflectionContext>(new ReflectionContext(GetType(), new PipelineSettingsBuilder().Build(), CultureInfo.InvariantCulture));
            var sut = CreateSut();

            // Act
            var result = sut.Resolve<ClassModel>(sourceObject);

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
            var result = sut.Resolve<ClassModel>(sourceObject);

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
            var result = sut.Resolve<ClassModel>(sourceObject);

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
            var result = sut.Resolve<ClassModel>(sourceObject);

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
            var result = sut.Resolve<ClassModelResolverTests>(sourceObject);

            // Assert
            result.Status.Should().Be(ResultStatus.Continue);
        }
    }
}

﻿namespace ClassFramework.Pipelines.Tests.Reflection.Features;

public class ValidationComponentTests : TestBase<Pipelines.Reflection.Features.ValidationComponent>
{
    public class Process : ValidationComponentTests
    {
        [Fact]
        public async Task Throws_On_Null_Context()
        {
            // Arrange
            var sut = CreateSut();

            // Act & Assert
            await sut.Awaiting(x => x.Process(context: null!, CancellationToken.None))
                     .Should().ThrowAsync<ArgumentNullException>().WithParameterName("context");
        }

        [Fact]
        public async Task Returns_Continue_When_Properties_Are_Found()
        {
            // Arrange
            var sourceModel = typeof(MyClass);
            var sut = CreateSut();
            var model = new ClassBuilder();
            var settings = CreateSettingsForReflection();
            var context = new PipelineContext<TypeBaseBuilder, ReflectionContext>(model, new ReflectionContext(sourceModel, settings.Build(), CultureInfo.InvariantCulture));

            // Act
            var result = await sut.Process(context, CancellationToken.None);

            // Assert
            result.Status.Should().Be(ResultStatus.Continue);
        }

        [Fact]
        public async Task Returns_Continue_When_Properties_Are_Not_Found_But_AllowGenerationWithoutProperties_Is_True()
        {
            // Arrange
            var sourceModel = typeof(MyClass);
            var sut = CreateSut();
            var model = new ClassBuilder();
            var settings = CreateSettingsForReflection(allowGenerationWithoutProperties: true);
            var context = new PipelineContext<TypeBaseBuilder, ReflectionContext>(model, new ReflectionContext(sourceModel, settings.Build(), CultureInfo.InvariantCulture));

            // Act
            var result = await sut.Process(context, CancellationToken.None);

            // Assert
            result.Status.Should().Be(ResultStatus.Continue);
        }
    }
}

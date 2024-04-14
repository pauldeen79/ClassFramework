﻿namespace ClassFramework.Pipelines.Tests.Entity.Components;

public class ValidationComponentTests : TestBase<Pipelines.Entity.Features.ValidationComponent>
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
            var sourceModel = CreateModel();
            var sut = CreateSut();
            var model = new ClassBuilder();
            var settings = CreateSettingsForEntity();
            var context = new PipelineContext<IConcreteTypeBuilder, EntityContext>(model, new EntityContext(sourceModel, settings.Build(), CultureInfo.InvariantCulture));

            // Act
            var result = await sut.Process(context, CancellationToken.None);

            // Assert
            result.Status.Should().Be(ResultStatus.Continue);
        }

        [Fact]
        public async Task Returns_Continue_When_Properties_Are_Not_Found_But_AllowGenerationWithoutProperties_Is_True()
        {
            // Arrange
            var sourceModel = new ClassBuilder().WithName("MyClass").BuildTyped();
            var sut = CreateSut();
            var model = new ClassBuilder();
            var settings = CreateSettingsForEntity(allowGenerationWithoutProperties: true);
            var context = new PipelineContext<IConcreteTypeBuilder, EntityContext>(model, new EntityContext(sourceModel, settings.Build(), CultureInfo.InvariantCulture));

            // Act
            var result = await sut.Process(context, CancellationToken.None);

            // Assert
            result.Status.Should().Be(ResultStatus.Continue);
        }

        [Fact]
        public async Task Returns_Continue_When_Properties_Are_Not_Found_But_EnableInheritance_Is_True()
        {
            // Arrange
            var sourceModel = new ClassBuilder().WithName("MyClass").BuildTyped();
            var sut = CreateSut();
            var model = new ClassBuilder();
            var settings = CreateSettingsForEntity(
                allowGenerationWithoutProperties: false,
                enableEntityInheritance: true);
            var context = new PipelineContext<IConcreteTypeBuilder, EntityContext>(model, new EntityContext(sourceModel, settings.Build(), CultureInfo.InvariantCulture));

            // Act
            var result = await sut.Process(context, CancellationToken.None);

            // Assert
            result.Status.Should().Be(ResultStatus.Continue);
        }
    }
}

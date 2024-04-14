﻿namespace ClassFramework.Pipelines.Tests.Entity.Components;

public class AbstractEntityComponentTests : TestBase<Pipelines.Entity.Features.AbstractEntityComponent>
{
    public class Process : AbstractEntityComponentTests
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
        public async Task Updates_IsAbstract_To_True_When_SourceModel_Is_Abstract()
        {
            // Arrange
            var sourceModel = CreateModel(baseClass: string.Empty);
            var sut = CreateSut();
            var model = new ClassBuilder().WithAbstract(false); // we want to make sure that the component updates the property
            var settings = CreateSettingsForEntity(
                enableEntityInheritance: true,
                isAbstract: true);
            var context = new PipelineContext<IConcreteTypeBuilder, EntityContext>(model, new EntityContext(sourceModel, settings.Build(), CultureInfo.InvariantCulture));

            // Act
            var result = await sut.Process(context, CancellationToken.None);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            model.Abstract.Should().BeTrue();
        }

        [Fact]
        public async Task Updates_IsAbstract_To_False_When_SourceModel_Is_Not_Abstract()
        {
            // Arrange
            var sourceModel = CreateModel(baseClass: string.Empty);
            var sut = CreateSut();
            var model = new ClassBuilder().WithAbstract(true); // we want to make sure that the component updates the property
            var settings = CreateSettingsForEntity(
                enableEntityInheritance: true,
                isAbstract: false);
            var context = new PipelineContext<IConcreteTypeBuilder, EntityContext>(model, new EntityContext(sourceModel, settings.Build(), CultureInfo.InvariantCulture));

            // Act
            var result = await sut.Process(context, CancellationToken.None);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            model.Abstract.Should().BeFalse();
        }

        [Fact]
        public async Task Returns_Success_When_Context_Model_Is_Not_Of_Type_ClassBuilder()
        {
            // Arrange
            var sourceModel = CreateModel(baseClass: string.Empty);
            var sut = CreateSut();
            var model = new StructBuilder(); // no ClassBuilder, so can't set Abstract
            var settings = CreateSettingsForEntity(
                enableEntityInheritance: true,
                isAbstract: true);
            var context = new PipelineContext<IConcreteTypeBuilder, EntityContext>(model, new EntityContext(sourceModel, settings.Build(), CultureInfo.InvariantCulture));

            // Act
            var result = await sut.Process(context, CancellationToken.None);

            // Assert
            result.IsSuccessful().Should().BeTrue();
        }
    }
}

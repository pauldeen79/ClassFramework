﻿namespace ClassFramework.Pipelines.Tests.Builder.Components;

public class GenericsComponentTests : TestBase<Pipelines.Builder.Features.GenericsComponent>
{
    public class Process : GenericsComponentTests
    {
        [Fact]
        public void Throws_On_Null_Context()
        {
            // Arrange
            var sut = CreateSut();

            // Act & Assert
            sut.Awaiting(x => x.Process(context: null!))
               .Should().ThrowAsync<ArgumentNullException>().WithParameterName("context");
        }

        [Fact]
        public async Task Adds_GenericTypeArguments()
        {
            // Arrange
            var sourceModel = new ClassBuilder().WithName("SomeClass").WithNamespace("SomeNamespace").AddGenericTypeArguments("T").Build();
            var sut = CreateSut();
            var model = new ClassBuilder();
            var settings = CreateSettingsForBuilder();
            var context = CreateContext(sourceModel, model, settings);

            // Act
            var result = await sut.Process(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            model.GenericTypeArguments.Should().BeEquivalentTo("T");
        }

        [Fact]
        public async Task Adds_GenericTypeArgumentConstraints()
        {
            // Arrange
            var sourceModel = new ClassBuilder().WithName("SomeClass").WithNamespace("SomeNamespace").AddGenericTypeArguments("T").AddGenericTypeArgumentConstraints("where T : class").Build();
            var sut = CreateSut();
            var model = new ClassBuilder();
            var settings = CreateSettingsForBuilder();
            var context = CreateContext(sourceModel, model, settings);

            // Act
            var result = await sut.Process(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            model.GenericTypeArgumentConstraints.Should().BeEquivalentTo("where T : class");
        }

        private static PipelineContext<BuilderContext, IConcreteTypeBuilder> CreateContext(TypeBase sourceModel, ClassBuilder model, PipelineSettingsBuilder settings)
            => new(new BuilderContext(sourceModel, settings.Build(), CultureInfo.InvariantCulture), model);
    }
}

﻿namespace ClassFramework.Pipelines.Tests.Entity.Components;

public class SetBaseClassComponentTests : TestBase<Pipelines.Entity.Features.SetBaseClassComponent>
{
    public class Process : SetBaseClassComponentTests
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
        public async Task Does_Not_Set_BaseClass_For_EntityInheritance_When_SourceModel_And_EntitySettings_Do_Not_Have_A_BaseClass()
        {
            // Arrange
            var sourceModel = CreateModel(baseClass: string.Empty);
            InitializeParser();
            var sut = CreateSut();
            var model = new ClassBuilder();
            var settings = CreateSettingsForEntity(
                baseClass:  null,
                enableEntityInheritance: true);
            var context = new PipelineContext<IConcreteTypeBuilder, EntityContext>(model, new EntityContext(sourceModel, settings.Build(), CultureInfo.InvariantCulture));

            // Act
            var result = await sut.Process(context, CancellationToken.None);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            model.BaseClass.Should().BeEmpty();
        }

        [Theory]
        [InlineData("")]
        [InlineData("ThisBaseClassGetsIgnored")]
        public async Task Sets_BaseClass_For_EntityInheritance_From_EntitySettings_When_Specified(string sourceModelBaseClass)
        {
            // Arrange
            var sourceModel = CreateModel(baseClass: sourceModelBaseClass);
            InitializeParser();
            var sut = CreateSut();
            var model = new ClassBuilder();
            var settings = CreateSettingsForEntity(
                baseClass: new ClassBuilder().WithName("MyBaseClass").WithNamespace("MyBaseNamespace").BuildTyped(),
                enableEntityInheritance: true);
            var context = new PipelineContext<IConcreteTypeBuilder, EntityContext>(model, new EntityContext(sourceModel, settings.Build(), CultureInfo.InvariantCulture));

            // Act
            var result = await sut.Process(context, CancellationToken.None);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            model.BaseClass.Should().Be("MyBaseNamespace.MyBaseClass");
        }

        [Fact]
        public async Task Sets_BaseClass_For_EntityInheritance_From_Source_When_Specified()
        {
            // Arrange
            var sourceModel = CreateModel(baseClass: "MyBaseNamespace.MyBaseClass");
            InitializeParser();
            var sut = CreateSut();
            var model = new ClassBuilder();
            var settings = CreateSettingsForEntity(
                baseClass: null,
                enableEntityInheritance: true);
            var context = new PipelineContext<IConcreteTypeBuilder, EntityContext>(model, new EntityContext(sourceModel, settings.Build(), CultureInfo.InvariantCulture));

            // Act
            var result = await sut.Process(context, CancellationToken.None);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            model.BaseClass.Should().Be("MyBaseNamespace.MyBaseClass");
        }
    }
}

﻿namespace ClassFramework.Pipelines.Tests.Entity.Components;

public class SetBaseClassComponentTests : TestBase<Pipelines.Entity.Components.SetBaseClassComponent>
{
    public class ProcessAsync : SetBaseClassComponentTests
    {
        [Fact]
        public void Throws_On_Null_Context()
        {
            // Arrange
            var sut = CreateSut();

            // Act & Assert
            sut.Awaiting(x => x.ProcessAsync(context: null!))
               .Should().ThrowAsync<ArgumentNullException>().WithParameterName("context");
        }

        [Fact]
        public async Task Does_Not_Set_BaseClass_For_EntityInheritance_When_SourceModel_And_EntitySettings_Do_Not_Have_A_BaseClass()
        {
            // Arrange
            var sourceModel = CreateClass(baseClass: string.Empty);
            InitializeParser();
            var sut = CreateSut();
            var settings = CreateSettingsForEntity(
                baseClass: null,
                enableEntityInheritance: true);
            var context = new PipelineContext<EntityContext>(new EntityContext(sourceModel, settings, CultureInfo.InvariantCulture));

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            context.Request.Builder.BaseClass.Should().BeEmpty();
        }

        [Theory]
        [InlineData("")]
        [InlineData("ThisBaseClassGetsIgnored")]
        public async Task Sets_BaseClass_For_EntityInheritance_From_EntitySettings_When_Specified(string sourceModelBaseClass)
        {
            // Arrange
            var sourceModel = CreateClass(baseClass: sourceModelBaseClass);
            InitializeParser();
            var sut = CreateSut();
            var settings = CreateSettingsForEntity(
                baseClass: new ClassBuilder().WithName("MyBaseClass").WithNamespace("MyBaseNamespace").BuildTyped(),
                enableEntityInheritance: true);
            var context = new PipelineContext<EntityContext>(new EntityContext(sourceModel, settings, CultureInfo.InvariantCulture));

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            context.Request.Builder.BaseClass.Should().Be("MyBaseNamespace.MyBaseClass");
        }

        [Fact]
        public async Task Sets_BaseClass_For_EntityInheritance_From_Source_When_Specified()
        {
            // Arrange
            var sourceModel = CreateClass(baseClass: "MyBaseNamespace.MyBaseClass");
            InitializeParser();
            var sut = CreateSut();
            var settings = CreateSettingsForEntity(
                baseClass: null,
                enableEntityInheritance: true);
            var context = new PipelineContext<EntityContext>(new EntityContext(sourceModel, settings, CultureInfo.InvariantCulture));

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            context.Request.Builder.BaseClass.Should().Be("MyBaseNamespace.MyBaseClass");
        }
    }
}

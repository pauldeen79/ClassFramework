namespace ClassFramework.Pipelines.Tests.Entity.Components;

public class ValidationComponentTests : TestBase<Pipelines.Entity.Components.ValidationComponent>
{
    public class Process : ValidationComponentTests
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
        public async Task Returns_Continue_When_Properties_Are_Found()
        {
            // Arrange
            var sourceModel = CreateClass();
            var sut = CreateSut();
            var settings = CreateSettingsForEntity();
            var context = new PipelineContext<EntityContext>(new EntityContext(sourceModel, settings.Build(), CultureInfo.InvariantCulture));

            // Act
            var result = await sut.Process(context);

            // Assert
            result.Status.Should().Be(ResultStatus.Continue);
        }

        [Fact]
        public async Task Returns_Continue_When_Properties_Are_Not_Found_But_AllowGenerationWithoutProperties_Is_True()
        {
            // Arrange
            var sourceModel = new ClassBuilder().WithName("MyClass").BuildTyped();
            var sut = CreateSut();
            var settings = CreateSettingsForEntity(allowGenerationWithoutProperties: true);
            var context = new PipelineContext<EntityContext>(new EntityContext(sourceModel, settings.Build(), CultureInfo.InvariantCulture));

            // Act
            var result = await sut.Process(context);

            // Assert
            result.Status.Should().Be(ResultStatus.Continue);
        }

        [Fact]
        public async Task Returns_Continue_When_Properties_Are_Not_Found_But_EnableInheritance_Is_True()
        {
            // Arrange
            var sourceModel = new ClassBuilder().WithName("MyClass").BuildTyped();
            var sut = CreateSut();
            var settings = CreateSettingsForEntity(
                allowGenerationWithoutProperties: false,
                enableEntityInheritance: true);
            var context = new PipelineContext<EntityContext>(new EntityContext(sourceModel, settings.Build(), CultureInfo.InvariantCulture));

            // Act
            var result = await sut.Process(context);

            // Assert
            result.Status.Should().Be(ResultStatus.Continue);
        }
    }
}

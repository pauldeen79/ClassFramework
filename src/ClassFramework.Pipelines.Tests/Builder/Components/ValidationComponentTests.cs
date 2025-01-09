namespace ClassFramework.Pipelines.Tests.Builder.Components;

public class ValidationComponentTests : TestBase<Pipelines.Builder.Components.ValidationComponent>
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
        public async Task Returns_Ok_When_Properties_Are_Found()
        {
            // Arrange
            var sourceModel = CreateClass();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder();
            var context = CreateContext(sourceModel, settings);

            // Act
            var result = await sut.Process(context);

            // Assert
            result.Status.Should().Be(ResultStatus.Ok);
        }

        [Fact]
        public async Task Returns_Ok_When_Properties_Are_Not_Found_But_AllowGenerationWithoutProperties_Is_True()
        {
            // Arrange
            var sourceModel = new ClassBuilder().WithName("MyClass").BuildTyped();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder(allowGenerationWithoutProperties: true);
            var context = CreateContext(sourceModel, settings);

            // Act
            var result = await sut.Process(context);

            // Assert
            result.Status.Should().Be(ResultStatus.Ok);
        }

        [Fact]
        public async Task Returns_Ok_When_Properties_Are_Not_Found_But_EnableInheritance_Is_True()
        {
            // Arrange
            var sourceModel = new ClassBuilder().WithName("MyClass").BuildTyped();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder(
                allowGenerationWithoutProperties: false,
                enableEntityInheritance: true);
            var context = CreateContext(sourceModel, settings);

            // Act
            var result = await sut.Process(context);

            // Assert
            result.Status.Should().Be(ResultStatus.Ok);
        }

        private static PipelineContext<BuilderContext> CreateContext(TypeBase sourceModel, PipelineSettingsBuilder settings)
            => new(new BuilderContext(sourceModel, settings.Build(), CultureInfo.InvariantCulture));
    }
}

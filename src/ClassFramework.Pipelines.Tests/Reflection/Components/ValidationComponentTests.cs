namespace ClassFramework.Pipelines.Tests.Reflection.Components;

public class ValidationComponentTests : TestBase<Pipelines.Reflection.Components.ValidationComponent>
{
    public class ProcessAsync : ValidationComponentTests
    {
        [Fact]
        public void Throws_On_Null_Context()
        {
            // Arrange
            var sut = CreateSut();

            // Act & Assert
            Action a = () => sut.ProcessAsync(context: null!);
            a.ShouldThrow<ArgumentNullException>().ParamName.ShouldBe("context");
        }

        [Fact]
        public async Task Returns_Ok_When_Properties_Are_Found()
        {
            // Arrange
            var sourceModel = typeof(MyClass);
            var sut = CreateSut();
            var settings = CreateSettingsForReflection();
            var context = new PipelineContext<ReflectionContext>(new ReflectionContext(sourceModel, settings, CultureInfo.InvariantCulture));

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.Status.ShouldBe(ResultStatus.Ok);
        }

        [Fact]
        public async Task Returns_Ok_When_Properties_Are_Not_Found_But_AllowGenerationWithoutProperties_Is_True()
        {
            // Arrange
            var sourceModel = typeof(MyClass);
            var sut = CreateSut();
            var settings = CreateSettingsForReflection(allowGenerationWithoutProperties: true);
            var context = new PipelineContext<ReflectionContext>(new ReflectionContext(sourceModel, settings, CultureInfo.InvariantCulture));

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.Status.ShouldBe(ResultStatus.Ok);
        }
    }
}

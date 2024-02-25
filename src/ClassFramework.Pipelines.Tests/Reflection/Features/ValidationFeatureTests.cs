namespace ClassFramework.Pipelines.Tests.Reflection.Features;

public class ValidationFeatureTests : TestBase<Pipelines.Reflection.Features.ValidationFeature>
{
    public class Process : ValidationFeatureTests
    {
        [Fact]
        public void Throws_On_Null_Context()
        {
            // Arrange
            var sut = CreateSut();

            // Act & Assert
            sut.Invoking(x => x.Process(context: null!))
               .Should().Throw<ArgumentNullException>().WithParameterName("context");
        }

        [Fact]
        public void Returns_Continue_When_Properties_Are_Found()
        {
            // Arrange
            var sourceModel = typeof(MyClass);
            var sut = CreateSut();
            var model = new ClassBuilder();
            var settings = CreateSettingsForReflection();
            var context = new PipelineContext<TypeBaseBuilder, ReflectionContext>(model, new ReflectionContext(sourceModel, settings.Build(), CultureInfo.InvariantCulture));

            // Act
            var result = sut.Process(context);

            // Assert
            result.Status.Should().Be(ResultStatus.Continue);
        }

        [Fact]
        public void Returns_Continue_When_Properties_Are_Not_Found_But_AllowGenerationWithoutProperties_Is_True()
        {
            // Arrange
            var sourceModel = typeof(MyClass);
            var sut = CreateSut();
            var model = new ClassBuilder();
            var settings = CreateSettingsForReflection(allowGenerationWithoutProperties: true);
            var context = new PipelineContext<TypeBaseBuilder, ReflectionContext>(model, new ReflectionContext(sourceModel, settings.Build(), CultureInfo.InvariantCulture));

            // Act
            var result = sut.Process(context);

            // Assert
            result.Status.Should().Be(ResultStatus.Continue);
        }
    }
}

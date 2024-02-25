namespace ClassFramework.Pipelines.Tests.Reflection.Features;

public class AddAttributesFeatureTests : TestBase<Pipelines.Reflection.Features.AddAttributesFeature>
{
    [ExcludeFromCodeCoverage] // just adding an attribute here, so we can use this class as source model in our tests
    public class Process : AddAttributesFeatureTests
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
        public void Adds_Attributes_When_CopyAttributePredicate_Setting_Is_Not_Null_And_CopyAttributes_Is_True()
        {
            // Arrange
            var sourceModel = GetType();
            var sut = CreateSut();
            var model = new ClassBuilder();
            var settings = CreateSettingsForReflection(copyAttributePredicate: _ => true, copyAttributes: true);
            var context = new PipelineContext<TypeBaseBuilder, ReflectionContext>(model, new ReflectionContext(sourceModel, settings.Build(), CultureInfo.InvariantCulture));

            // Act
            var result = sut.Process(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            model.Attributes.Should().BeEquivalentTo(new[] { new AttributeBuilder().WithName("System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute") });
        }

        [Fact]
        public void Adds_Attributes_When_CopyAttributePredicate_Setting_Is_Null_And_CopyAttributes_Is_True()
        {
            // Arrange
            var sourceModel = GetType();
            var sut = CreateSut();
            var model = new ClassBuilder();
            var settings = CreateSettingsForReflection(copyAttributePredicate: null, copyAttributes: true);
            var context = new PipelineContext<TypeBaseBuilder, ReflectionContext>(model, new ReflectionContext(sourceModel, settings.Build(), CultureInfo.InvariantCulture));

            // Act
            var result = sut.Process(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            model.Attributes.Should().BeEquivalentTo(new[] { new AttributeBuilder().WithName("System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute") });
        }

        [Fact]
        public void Does_Not_Copy_Attributes_When_CopyAttributes_Is_False()
        {
            // Arrange
            var sourceModel = GetType();
            var sut = CreateSut();
            var model = new ClassBuilder();
            var settings = CreateSettingsForReflection(copyAttributes: false);
            var context = new PipelineContext<TypeBaseBuilder, ReflectionContext>(model, new ReflectionContext(sourceModel, settings.Build(), CultureInfo.InvariantCulture));

            // Act
            var result = sut.Process(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            model.Attributes.Should().BeEmpty();
        }
    }
}

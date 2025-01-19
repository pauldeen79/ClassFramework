namespace ClassFramework.Pipelines.Tests.Reflection.Components;

public class AddAttributesComponentTests : TestBase<Pipelines.Reflection.Components.AddAttributesComponent>
{
    [ExcludeFromCodeCoverage] // just adding an attribute here, so we can use this class as source model in our tests
    public class Process : AddAttributesComponentTests
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
        public async Task Adds_Attributes_When_CopyAttributePredicate_Setting_Is_Not_Null_And_CopyAttributes_Is_True()
        {
            // Arrange
            var sourceModel = GetType();
            var sut = CreateSut();
            var settings = CreateSettingsForReflection(copyAttributePredicate: _ => true, copyAttributes: true);
            var context = new PipelineContext<ReflectionContext>(new ReflectionContext(sourceModel, settings.Build(), CultureInfo.InvariantCulture));

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            context.Request.Builder.Attributes.Should().BeEquivalentTo([new AttributeBuilder().WithName("System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute")]);
        }

        [Fact]
        public async Task Adds_Attributes_When_CopyAttributePredicate_Setting_Is_Null_And_CopyAttributes_Is_True()
        {
            // Arrange
            var sourceModel = GetType();
            var sut = CreateSut();
            var settings = CreateSettingsForReflection(copyAttributePredicate: null, copyAttributes: true);
            var context = new PipelineContext<ReflectionContext>(new ReflectionContext(sourceModel, settings.Build(), CultureInfo.InvariantCulture));

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            context.Request.Builder.Attributes.Should().BeEquivalentTo([new AttributeBuilder().WithName("System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute")]);
        }

        [Fact]
        public async Task Does_Not_Copy_Attributes_When_CopyAttributes_Is_False()
        {
            // Arrange
            var sourceModel = GetType();
            var sut = CreateSut();
            var settings = CreateSettingsForReflection(copyAttributes: false);
            var context = new PipelineContext<ReflectionContext>(new ReflectionContext(sourceModel, settings.Build(), CultureInfo.InvariantCulture));

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            context.Request.Builder.Attributes.Should().BeEmpty();
        }
    }
}

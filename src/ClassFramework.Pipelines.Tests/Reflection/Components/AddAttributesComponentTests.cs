namespace ClassFramework.Pipelines.Tests.Reflection.Components;

public class AddAttributesComponentTests : TestBase<Pipelines.Reflection.Components.AddAttributesComponent>
{
    [ExcludeFromCodeCoverage] // just adding an attribute here, so we can use this class as source model in our tests
    public class ExecuteAsync : AddAttributesComponentTests
    {
        [Fact]
        public async Task Throws_On_Null_Context()
        {
            // Arrange
            var sut = CreateSut();

            // Act & Assert
            Task a = sut.ExecuteAsync(context: null!, CommandService, CancellationToken.None);
            (await a.ShouldThrowAsync<ArgumentNullException>())
                .ParamName.ShouldBe("context");
        }

        [Fact]
        public async Task Adds_Attributes_When_CopyAttributePredicate_Setting_Is_Not_Null_And_CopyAttributes_Is_True()
        {
            // Arrange
            var sourceModel = GetType();
            var sut = CreateSut();
            var settings = CreateSettingsForReflection(copyAttributePredicate: _ => true, copyAttributes: true);
            var context = new ReflectionContext(sourceModel, settings, CultureInfo.InvariantCulture, CancellationToken.None);

            // Act
            var result = await sut.ExecuteAsync(context, CommandService, CancellationToken.None);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            context.Builder.Attributes.ToArray().ShouldBeEquivalentTo(new[] { new AttributeBuilder().WithName("System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute") });
        }

        [Fact]
        public async Task Adds_Attributes_When_CopyAttributePredicate_Setting_Is_Null_And_CopyAttributes_Is_True()
        {
            // Arrange
            var sourceModel = GetType();
            var sut = CreateSut();
            var settings = CreateSettingsForReflection(copyAttributePredicate: null, copyAttributes: true);
            var context = new ReflectionContext(sourceModel, settings, CultureInfo.InvariantCulture, CancellationToken.None);

            // Act
            var result = await sut.ExecuteAsync(context, CommandService, CancellationToken.None);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            context.Builder.Attributes.ToArray().ShouldBeEquivalentTo(new[] { new AttributeBuilder().WithName("System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute") });
        }

        [Fact]
        public async Task Does_Not_Copy_Attributes_When_CopyAttributes_Is_False()
        {
            // Arrange
            var sourceModel = GetType();
            var sut = CreateSut();
            var settings = CreateSettingsForReflection(copyAttributes: false);
            var context = new ReflectionContext(sourceModel, settings, CultureInfo.InvariantCulture, CancellationToken.None);

            // Act
            var result = await sut.ExecuteAsync(context, CommandService, CancellationToken.None);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            context.Builder.Attributes.ShouldBeEmpty();
        }
    }
}

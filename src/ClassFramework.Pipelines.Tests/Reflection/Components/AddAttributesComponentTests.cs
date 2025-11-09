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
            var response = new ClassBuilder();

            // Act & Assert
            Task a = sut.ExecuteAsync(context: null!, response, CommandService, CancellationToken.None);
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
            var command = new GenerateTypeFromReflectionCommand(sourceModel, settings, CultureInfo.InvariantCulture);
            var response = new ClassBuilder();

            // Act
            var result = await sut.ExecuteAsync(command, response, CommandService, CancellationToken.None);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            response.Attributes.ToArray().ShouldBeEquivalentTo(new[] { new AttributeBuilder().WithName("System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute") });
        }

        [Fact]
        public async Task Adds_Attributes_When_CopyAttributePredicate_Setting_Is_Null_And_CopyAttributes_Is_True()
        {
            // Arrange
            var sourceModel = GetType();
            var sut = CreateSut();
            var settings = CreateSettingsForReflection(copyAttributePredicate: null, copyAttributes: true);
            var command = new GenerateTypeFromReflectionCommand(sourceModel, settings, CultureInfo.InvariantCulture);
            var response = new ClassBuilder();

            // Act
            var result = await sut.ExecuteAsync(command, response, CommandService, CancellationToken.None);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            response.Attributes.ToArray().ShouldBeEquivalentTo(new[] { new AttributeBuilder().WithName("System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute") });
        }

        [Fact]
        public async Task Does_Not_Copy_Attributes_When_CopyAttributes_Is_False()
        {
            // Arrange
            var sourceModel = GetType();
            var sut = CreateSut();
            var settings = CreateSettingsForReflection(copyAttributes: false);
            var command = new GenerateTypeFromReflectionCommand(sourceModel, settings, CultureInfo.InvariantCulture);
            var response = new ClassBuilder();

            // Act
            var result = await sut.ExecuteAsync(command, response, CommandService, CancellationToken.None);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            response.Attributes.ShouldBeEmpty();
        }
    }
}

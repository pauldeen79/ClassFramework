namespace ClassFramework.Pipelines.Tests.Entity.Components;

public class AddAttributesComponentTests : TestBase<Pipelines.Entity.Components.AddAttributesComponent>
{
    public class ExecuteAsync : AddAttributesComponentTests
    {
        [Fact]
        public async Task Throws_On_Null_Command()
        {
            // Arrange
            var sut = CreateSut();
            var response = new ClassBuilder();

            // Act & Assert
            Task a = sut.ExecuteAsync(command: null!, response, CommandService, CancellationToken.None);
            (await a.ShouldThrowAsync<ArgumentNullException>())
             .ParamName.ShouldBe("command");
        }

        [Fact]
        public async Task Adds_Attributes_When_CopyAttributePredicate_Setting_Is_Not_Null_And_CopyAttributes_Is_True()
        {
            // Arrange
            var sourceModel = new ClassBuilder().WithName("SomeClass").WithNamespace("SomeNamespace").AddAttributes(new AttributeBuilder().WithName("MyAttribute")).BuildTyped();
            var sut = CreateSut();
            var settings = CreateSettingsForEntity(copyAttributePredicate: _ => true, copyAttributes: true);
            var command = new GenerateEntityCommand(sourceModel, settings, CultureInfo.InvariantCulture);
            var response = new ClassBuilder();

            // Act
            var result = await sut.ExecuteAsync(command, response, CommandService, CancellationToken.None);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            response.Attributes.ToArray().ShouldBeEquivalentTo(new[] { new AttributeBuilder().WithName("MyAttribute") });
        }

        [Fact]
        public async Task Adds_Attributes_When_CopyAttributePredicate_Setting_Is_Null_And_CopyAttributes_Is_True()
        {
            // Arrange
            var sourceModel = new ClassBuilder().WithName("SomeClass").WithNamespace("SomeNamespace").AddAttributes(new AttributeBuilder().WithName("MyAttribute")).BuildTyped();
            var sut = CreateSut();
            var settings = CreateSettingsForEntity(copyAttributePredicate: null, copyAttributes: true);
            var command = new GenerateEntityCommand(sourceModel, settings, CultureInfo.InvariantCulture);
            var response = new ClassBuilder();

            // Act
            var result = await sut.ExecuteAsync(command, response, CommandService, CancellationToken.None);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            response.Attributes.ToArray().ShouldBeEquivalentTo(new[] { new AttributeBuilder().WithName("MyAttribute") });
        }

        [Fact]
        public async Task Does_Not_Copy_Attributes_When_CopyAttributes_Is_False()
        {
            // Arrange
            var sourceModel = new ClassBuilder().WithName("SomeClass").WithNamespace("SomeNamespace").AddAttributes(new AttributeBuilder().WithName("MyAttribute")).BuildTyped();
            var sut = CreateSut();
            var settings = CreateSettingsForEntity(copyAttributes: false);
            var command = new GenerateEntityCommand(sourceModel, settings, CultureInfo.InvariantCulture);
            var response = new ClassBuilder();

            // Act
            var result = await sut.ExecuteAsync(command, response, CommandService, CancellationToken.None);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            response.Attributes.ShouldBeEmpty();
        }
    }
}

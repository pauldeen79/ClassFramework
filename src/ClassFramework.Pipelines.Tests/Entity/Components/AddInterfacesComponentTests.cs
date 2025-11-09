namespace ClassFramework.Pipelines.Tests.Entity.Components;

public class AddInterfacesComponentTests : TestBase<Pipelines.Entity.Components.AddInterfacesComponent>
{
    public class ExecuteAsync : AddInterfacesComponentTests
    {
        [Fact]
        public async Task Throws_On_Null_Context()
        {
            // Arrange
            var sut = CreateSut();
            var response = new ClassBuilder();

            // Act & Assert
            var t = sut.ExecuteAsync(context: null!, response, CommandService, CancellationToken.None);
            (await Should.ThrowAsync<ArgumentNullException>(t))
             .ParamName.ShouldBe("context");
        }

        [Fact]
        public async Task Adds_Interfaces_When_CopyInterfacePredicate_Setting_Is_Not_Null_And_CopyAttributes_Is_True()
        {
            // Arrange
            var sourceModel = new ClassBuilder().WithName("SomeClass").WithNamespace("SomeNamespace").AddInterfaces("IMyInterface").BuildTyped();
            var sut = CreateSut();
            var settings = CreateSettingsForEntity(copyInterfacePredicate: _ => true, copyInterfaces: true);
            var command = new GenerateEntityCommand(sourceModel, settings, CultureInfo.InvariantCulture);
            var response = new ClassBuilder();

            // Act
            var result = await sut.ExecuteAsync(command, response, CommandService, CancellationToken.None);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            response.Interfaces.ToArray().ShouldBeEquivalentTo(new[] { "IMyInterface" });
        }

        [Fact]
        public async Task Adds_Interfaces_When_CopyInterfacePredicate_Setting_Is_Null_And_CopyAttributes_Is_True()
        {
            // Arrange
            var sourceModel = new ClassBuilder().WithName("SomeClass").WithNamespace("SomeNamespace").AddInterfaces("IMyInterface").BuildTyped();
            var sut = CreateSut();
            var settings = CreateSettingsForEntity(copyInterfacePredicate: null, copyInterfaces: true);
            var command = new GenerateEntityCommand(sourceModel, settings, CultureInfo.InvariantCulture);
            var response = new ClassBuilder();

            // Act
            var result = await sut.ExecuteAsync(command, response, CommandService, CancellationToken.None);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            response.Interfaces.ToArray().ShouldBeEquivalentTo(new[] { "IMyInterface" });
        }

        [Fact]
        public async Task Does_Not_Add_Interfaces_When_And_CopyAttributes_Is_False()
        {
            // Arrange
            var sourceModel = new ClassBuilder().WithName("SomeClass").WithNamespace("SomeNamespace").AddInterfaces("IMyInterface").BuildTyped();
            var sut = CreateSut();
            var settings = CreateSettingsForEntity(copyInterfaces: false);
            var command = new GenerateEntityCommand(sourceModel, settings, CultureInfo.InvariantCulture);
            var response = new ClassBuilder();

            // Act
            var result = await sut.ExecuteAsync(command, response, CommandService, CancellationToken.None);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            response.Interfaces.ShouldBeEmpty();
        }
    }
}

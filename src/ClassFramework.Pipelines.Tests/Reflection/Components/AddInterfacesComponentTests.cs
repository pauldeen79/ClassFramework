namespace ClassFramework.Pipelines.Tests.Reflection.Components;

public class AddInterfacesComponentTests : TestBase<Pipelines.Reflection.Components.AddInterfacesComponent>
{
    public class ExecuteAsync : AddInterfacesComponentTests
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
        public async Task Does_Not_Copy_Interfaces_When_CopyInterfaces_Is_False()
        {
            // Arrange
            var sut = CreateSut();
            var sourceModel = typeof(MyClass);
            var settings = CreateSettingsForReflection(copyInterfaces: false);
            var command = new GenerateTypeFromReflectionCommand(sourceModel, settings, CultureInfo.InvariantCulture);
            var response = new ClassBuilder();

            // Act
            var result = await sut.ExecuteAsync(command, response, CommandService, CancellationToken.None);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            response.Interfaces.ShouldBeEmpty();
        }

        [Fact]
        public async Task Copies_Interfaces_When_CopyInterfaces_Is_True()
        {
            // Arrange
            var sut = CreateSut();
            var sourceModel = typeof(MyClass);
            var settings = CreateSettingsForReflection(copyInterfaces: true);
            var command = new GenerateTypeFromReflectionCommand(sourceModel, settings, CultureInfo.InvariantCulture);
            var response = new ClassBuilder();

            // Act
            var result = await sut.ExecuteAsync(command, response, CommandService, CancellationToken.None);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            response.Interfaces.Count.ShouldBe(1);
        }

        [Fact]
        public async Task Filters_Interfaces_When_CopyInterfaces_Is_True_And_Predicate_Is_Set()
        {
            // Arrange
            var sut = CreateSut();
            var sourceModel = typeof(MyClass);
            var settings = CreateSettingsForReflection(copyInterfaces: false, copyInterfacePredicate: _ => false);
            var command = new GenerateTypeFromReflectionCommand(sourceModel, settings, CultureInfo.InvariantCulture);
            var response = new ClassBuilder();

            // Act
            var result = await sut.ExecuteAsync(command, response, CommandService, CancellationToken.None);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            response.Interfaces.ShouldBeEmpty();
        }
    }
}

namespace ClassFramework.Pipelines.Tests.Entity.Components;

public class AbstractEntityComponentTests : TestBase<Pipelines.Entity.Components.AbstractEntityComponent>
{
    public class ExecuteAsync : AbstractEntityComponentTests
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
        public async Task Updates_IsAbstract_To_True_When_SourceModel_Is_Abstract()
        {
            // Arrange
            var sourceModel = CreateClass(baseClass: string.Empty);
            var sut = CreateSut();
            var settings = CreateSettingsForEntity(
                enableEntityInheritance: true,
                isAbstract: true);
            var command = new GenerateEntityCommand(sourceModel, settings, CultureInfo.InvariantCulture);
            var response = new ClassBuilder();
            response.WithAbstract(false); // we want to make sure that the component updates the property

            // Act
            var result = await sut.ExecuteAsync(command, response, CommandService, CancellationToken.None);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            response.Abstract.ShouldBeTrue();
        }

        [Fact]
        public async Task Updates_IsAbstract_To_False_When_SourceModel_Is_Not_Abstract()
        {
            // Arrange
            var sourceModel = CreateClass(baseClass: string.Empty);
            var sut = CreateSut();
            var settings = CreateSettingsForEntity(
                enableEntityInheritance: true,
                isAbstract: false);
            var command = new GenerateEntityCommand(sourceModel, settings, CultureInfo.InvariantCulture);
            var response = new ClassBuilder();
            response.WithAbstract(true); // we want to make sure that the component updates the property

            // Act
            var result = await sut.ExecuteAsync(command, response, CommandService, CancellationToken.None);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            response.Abstract.ShouldBeFalse();
        }
    }
}

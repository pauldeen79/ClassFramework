namespace ClassFramework.Pipelines.Tests.Reflection.Components;

public class AddMethodsComponentTests : TestBase<Pipelines.Reflection.Components.AddMethodsComponent>
{
    public class ExecuteAsync : AddMethodsComponentTests
    {
        [Fact]
        public async Task Throws_On_Null_Context()
        {
            // Arrange
            var sut = CreateSut();
            var response = new ClassBuilder();

            // Act & Assert
            Task a = sut.ExecuteAsync(context: null!, response, CommandService, CancellationToken.None);
            (await a.ShouldThrowAsync<ArgumentNullException>()).ParamName.ShouldBe("context");
        }

        [Fact]
        public async Task Copies_Methods()
        {
            // Arrange
            var sut = CreateSut();
            var sourceModel = typeof(MyClass);
            var settings = CreateSettingsForReflection();
            var command = new GenerateTypeFromReflectionCommand(sourceModel, settings, CultureInfo.InvariantCulture, CancellationToken.None);
            var response = new ClassBuilder();

            // Act
            var result = await sut.ExecuteAsync(command, response, CommandService, CancellationToken.None);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            response.Methods.Count.ShouldBe(1);
        }
    }
}

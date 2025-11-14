namespace ClassFramework.Pipelines.Tests.Entity.Components;

public class AddGenericsComponentTests : TestBase<Pipelines.Entity.Components.AddGenericsComponent>
{
    public class ExecuteAsync : AddGenericsComponentTests
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
        public async Task Adds_Generics_When_Available()
        {
            // Arrange
            var sourceModel = CreateGenericClass(addProperties: false);
            var sut = CreateSut();
            var settings = CreateSettingsForEntity();
            var command = new GenerateEntityCommand(sourceModel, settings, CultureInfo.InvariantCulture);
            var response = new ClassBuilder();

            // Act
            var result = await sut.ExecuteAsync(command, response, CommandService, CancellationToken.None);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            response.GenericTypeArguments.ToArray().ShouldBeEquivalentTo(new[] { "T" });
            response.GenericTypeArgumentConstraints.ToArray().ShouldBeEquivalentTo(new[] { "where T : class" });
        }
    }
}

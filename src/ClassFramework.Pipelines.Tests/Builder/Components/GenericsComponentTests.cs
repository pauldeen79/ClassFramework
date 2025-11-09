namespace ClassFramework.Pipelines.Tests.Builder.Components;

public class GenericsComponentTests : TestBase<Pipelines.Builder.Components.GenericsComponent>
{
    public class ExecuteAsync : GenericsComponentTests
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
        public async Task Adds_GenericTypeArguments()
        {
            // Arrange
            var sourceModel = new ClassBuilder()
                .WithName("SomeClass")
                .WithNamespace("SomeNamespace")
                .AddGenericTypeArguments("T")
                .Build();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder();
            var command = CreateCommand(sourceModel, settings);
            var response = new ClassBuilder();

            // Act
            var result = await sut.ExecuteAsync(command, response, CommandService, CancellationToken.None);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            response.GenericTypeArguments.ToArray().ShouldBeEquivalentTo(new[] { "T" });
        }

        [Fact]
        public async Task Adds_GenericTypeArgumentConstraints()
        {
            // Arrange
            var sourceModel = new ClassBuilder()
                .WithName("SomeClass")
                .WithNamespace("SomeNamespace")
                .AddGenericTypeArguments("T")
                .AddGenericTypeArgumentConstraints("where T : class")
                .Build();
            var sut = CreateSut();
            var settings = CreateSettingsForBuilder();
            var command = CreateCommand(sourceModel, settings);
            var response = new ClassBuilder();

            // Act
            var result = await sut.ExecuteAsync(command, response, CommandService, CancellationToken.None);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            response.GenericTypeArgumentConstraints.ToArray().ShouldBeEquivalentTo(new[] { "where T : class" });
        }

        private static GenerateBuilderCommand CreateCommand(TypeBase sourceModel, PipelineSettingsBuilder settings)
            => new GenerateBuilderCommand(sourceModel, settings, CultureInfo.InvariantCulture);
    }
}

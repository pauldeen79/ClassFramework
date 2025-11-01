namespace ClassFramework.Pipelines.Tests.Builder.Components;

public class GenericsComponentTests : TestBase<Pipelines.Builder.Components.GenericsComponent>
{
    public class ExecuteAsync : GenericsComponentTests
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
            var context = CreateContext(sourceModel, settings);

            // Act
            var result = await sut.ExecuteAsync(context, CommandService, CancellationToken.None);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            context.Builder.GenericTypeArguments.ToArray().ShouldBeEquivalentTo(new[] { "T" });
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
            var context = CreateContext(sourceModel, settings);

            // Act
            var result = await sut.ExecuteAsync(context, CommandService, CancellationToken.None);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            context.Builder.GenericTypeArgumentConstraints.ToArray().ShouldBeEquivalentTo(new[] { "where T : class" });
        }

        private static BuilderContext CreateContext(TypeBase sourceModel, PipelineSettingsBuilder settings)
            => new BuilderContext(sourceModel, settings, CultureInfo.InvariantCulture, CancellationToken.None);
    }
}

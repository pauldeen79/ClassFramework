namespace ClassFramework.Pipelines.Tests.Entity.Components;

public class AddGenericsComponentTests : TestBase<Pipelines.Entity.Components.AddGenericsComponent>
{
    public class ExecuteAsync : AddGenericsComponentTests
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
        public async Task Adds_Generics_When_Available()
        {
            // Arrange
            var sourceModel = CreateGenericClass(addProperties: false);
            var sut = CreateSut();
            var settings = CreateSettingsForEntity();
            var context = new EntityContext(sourceModel, settings, CultureInfo.InvariantCulture, CancellationToken.None);

            // Act
            var result = await sut.ExecuteAsync(context, CommandService, CancellationToken.None);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            context.Builder.GenericTypeArguments.ToArray().ShouldBeEquivalentTo(new[] { "T" });
            context.Builder.GenericTypeArgumentConstraints.ToArray().ShouldBeEquivalentTo(new[] { "where T : class" });
        }
    }
}

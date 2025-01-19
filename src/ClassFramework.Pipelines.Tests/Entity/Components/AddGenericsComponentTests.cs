namespace ClassFramework.Pipelines.Tests.Entity.Components;

public class AddGenericsComponentTests : TestBase<Pipelines.Entity.Components.AddGenericsComponent>
{
    public class Process : AddGenericsComponentTests
    {
        [Fact]
        public void Throws_On_Null_Context()
        {
            // Arrange
            var sut = CreateSut();

            // Act & Assert
            sut.Awaiting(x => x.ProcessAsync(context: null!))
               .Should().ThrowAsync<ArgumentNullException>().WithParameterName("context");
        }

        [Fact]
        public async Task Adds_Generics_When_Available()
        {
            // Arrange
            var sourceModel = CreateGenericClass(addProperties: false);
            var sut = CreateSut();
            var settings = CreateSettingsForEntity();
            var context = new PipelineContext<EntityContext>(new EntityContext(sourceModel, settings.Build(), CultureInfo.InvariantCulture));

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            context.Request.Builder.GenericTypeArguments.Should().BeEquivalentTo("T");
            context.Request.Builder.GenericTypeArgumentConstraints.Should().BeEquivalentTo("where T : class");
        }
    }
}

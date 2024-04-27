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
            sut.Awaiting(x => x.Process(context: null!))
               .Should().ThrowAsync<ArgumentNullException>().WithParameterName("context");
        }

        [Fact]
        public async Task Adds_Generics_When_Available()
        {
            // Arrange
            var sourceModel = CreateGenericModel(addProperties: false);
            var sut = CreateSut();
            var model = new ClassBuilder();
            var settings = CreateSettingsForEntity();
            var context = new PipelineContext<EntityContext, IConcreteTypeBuilder>(new EntityContext(sourceModel, settings.Build(), CultureInfo.InvariantCulture), model);

            // Act
            var result = await sut.Process(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            model.GenericTypeArguments.Should().BeEquivalentTo("T");
            model.GenericTypeArgumentConstraints.Should().BeEquivalentTo("where T : class");
        }
    }
}

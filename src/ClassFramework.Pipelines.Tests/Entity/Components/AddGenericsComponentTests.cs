namespace ClassFramework.Pipelines.Tests.Entity.Components;

public class AddGenericsComponentTests : TestBase<Pipelines.Entity.Features.AddGenericsComponent>
{
    public class Process : AddGenericsComponentTests
    {
        [Fact]
        public async Task Throws_On_Null_Context()
        {
            // Arrange
            var sut = CreateSut();

            // Act & Assert
            await sut.Awaiting(x => x.Process(context: null!))
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
            var context = new PipelineContext<IConcreteTypeBuilder, EntityContext>(model, new EntityContext(sourceModel, settings.Build(), CultureInfo.InvariantCulture));

            // Act
            var result = await sut.Process(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            model.GenericTypeArguments.Should().BeEquivalentTo("T");
            model.GenericTypeArgumentConstraints.Should().BeEquivalentTo("where T : class");
        }
    }
}

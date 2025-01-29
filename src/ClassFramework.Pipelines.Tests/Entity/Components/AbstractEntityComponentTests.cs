namespace ClassFramework.Pipelines.Tests.Entity.Components;

public class AbstractEntityComponentTests : TestBase<Pipelines.Entity.Components.AbstractEntityComponent>
{
    public class ProcessAsync : AbstractEntityComponentTests
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
        public async Task Updates_IsAbstract_To_True_When_SourceModel_Is_Abstract()
        {
            // Arrange
            var sourceModel = CreateClass(baseClass: string.Empty);
            var sut = CreateSut();
            var settings = CreateSettingsForEntity(
                enableEntityInheritance: true,
                isAbstract: true);
            var context = new PipelineContext<EntityContext>(new EntityContext(sourceModel, settings, CultureInfo.InvariantCulture));
            context.Request.Builder.WithAbstract(false); // we want to make sure that the component updates the property

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            context.Request.Builder.Abstract.Should().BeTrue();
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
            var context = new PipelineContext<EntityContext>(new EntityContext(sourceModel, settings, CultureInfo.InvariantCulture));
            context.Request.Builder.WithAbstract(true); // we want to make sure that the component updates the property

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().Should().BeTrue();
            context.Request.Builder.Abstract.Should().BeFalse();
        }
    }
}

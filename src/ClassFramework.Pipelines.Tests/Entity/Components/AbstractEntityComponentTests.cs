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

            // Act & Assert
            Task a = sut.ExecuteAsync(context: null!, CommandService, CancellationToken.None);
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
            var context = new EntityContext(sourceModel, settings, CultureInfo.InvariantCulture, CancellationToken.None);
            context.Builder.WithAbstract(false); // we want to make sure that the component updates the property

            // Act
            var result = await sut.ExecuteAsync(context, CommandService, CancellationToken.None);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            context.Builder.Abstract.ShouldBeTrue();
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
            var context = new EntityContext(sourceModel, settings, CultureInfo.InvariantCulture, CancellationToken.None);
            context.Builder.WithAbstract(true); // we want to make sure that the component updates the property

            // Act
            var result = await sut.ExecuteAsync(context, CommandService, CancellationToken.None);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            context.Builder.Abstract.ShouldBeFalse();
        }
    }
}

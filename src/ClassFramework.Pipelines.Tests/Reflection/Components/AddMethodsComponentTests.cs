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

            // Act & Assert
            Task a = sut.ExecuteAsync(context: null!, CommandService, CancellationToken.None);
            (await a.ShouldThrowAsync<ArgumentNullException>()).ParamName.ShouldBe("context");
        }

        [Fact]
        public async Task Copies_Methods()
        {
            // Arrange
            var sut = CreateSut();
            var sourceModel = typeof(MyClass);
            var settings = CreateSettingsForReflection();
            var context = new ReflectionContext(sourceModel, settings, CultureInfo.InvariantCulture, CancellationToken.None);

            // Act
            var result = await sut.ExecuteAsync(context, CommandService, CancellationToken.None);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            context.Builder.Methods.Count.ShouldBe(1);
        }
    }
}

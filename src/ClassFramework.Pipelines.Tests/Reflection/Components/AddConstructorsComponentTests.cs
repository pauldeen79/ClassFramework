namespace ClassFramework.Pipelines.Tests.Reflection.Components;

public class AddConstructorsComponentTests : TestBase<Pipelines.Reflection.Components.AddConstructorsComponent>
{
    public class ExecuteAsync : AddConstructorsComponentTests
    {
        [Fact]
        public async Task Throws_On_Null_Context()
        {
            // Arrange
            var sut = CreateSut();
            var response = new ClassBuilder();

            // Act & Assert
            Task a = sut.ExecuteAsync(context: null!, response, CommandService, CancellationToken.None);
            (await a.ShouldThrowAsync<ArgumentNullException>())
                .ParamName.ShouldBe("context");
        }

        [Fact]
        public async Task Does_Not_Add_Constructors_When_CreateConstructors_Is_Set_To_False()
        {
            // Arrange
            var sut = CreateSut();
            var sourceModel = typeof(MyConstructorTestClass);
            var settings = CreateSettingsForReflection(createConstructors: false);
            var command = new GenerateTypeFromReflectionCommand(sourceModel, settings, CultureInfo.InvariantCulture);
            var response = new ClassBuilder();

            // Act
            var result = await sut.ExecuteAsync(command, response, CommandService, CancellationToken.None);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            response.GetConstructors().ShouldBeEmpty();
        }

        [Fact]
        public async Task Does_Not_Add_Constructors_When_SourceModel_Is_Of_Type_Interface()
        {
            // Arrange
            var sut = CreateSut();
            var sourceModel = typeof(MyConstructorTestClass);
            var settings = CreateSettingsForReflection(createConstructors: false);
            var command = new GenerateTypeFromReflectionCommand(sourceModel, settings, CultureInfo.InvariantCulture);
            var response = new ClassBuilder();

            // Act
            var result = await sut.ExecuteAsync(command, response, CommandService, CancellationToken.None);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            // can't even check constructors on model, because an interface does not have constructors
        }

        [Fact]
        public async Task Adds_Constructors_When_CreateConstructors_Is_Set_To_True_And_SourceModel_Has_Constructors()
        {
            // Arrange
            var sut = CreateSut();
            var sourceModel = typeof(MyConstructorTestClass);
            var settings = CreateSettingsForReflection(createConstructors: true);
            var command = new GenerateTypeFromReflectionCommand(sourceModel, settings, CultureInfo.InvariantCulture);
            var response = new ClassBuilder();

            // Act
            var result = await sut.ExecuteAsync(command, response, CommandService, CancellationToken.None);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            response.GetConstructors().Count.ShouldBe(2);
            response.GetConstructors().First().Parameters.ShouldBeEmpty();
            response.GetConstructors().Last().Parameters.Select(x => x.Name).ToArray().ShouldBeEquivalentTo(new[] { "value" });
            response.GetConstructors().Last().Parameters.Select(x => x.TypeName).ToArray().ShouldBeEquivalentTo(new[] { "System.Int32" });
        }
    }
}

#pragma warning disable CA1812 // Avoid uninstantiated internal classes
internal sealed class MyConstructorTestClass
#pragma warning restore CA1812 // Avoid uninstantiated internal classes
{
    public MyConstructorTestClass()
    {
    }

    public MyConstructorTestClass(int value)
    {
    }
}

internal interface IMyConstructorTestInterface
{
}
